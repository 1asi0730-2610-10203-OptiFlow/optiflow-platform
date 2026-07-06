using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using optiflow_platform.IAM.Infrastructure.Pipeline.Middleware.Attributes;
using optiflow_platform.Shared.Application.Patterns;
using optiflow_platform.Shared.Application.Services;
using optiflow_platform.Subscription.Application.Internal.OutboundServices.Stripe;
using optiflow_platform.Subscription.Application.Services;
using optiflow_platform.Subscription.Domain.Model.Commands;
using optiflow_platform.Subscription.Domain.Model.Queries;
using optiflow_platform.Subscription.Domain.Model.ValueObjects;
using optiflow_platform.Subscription.Domain.Repositories;
using optiflow_platform.Subscription.Interfaces.REST.Resources;
using Swashbuckle.AspNetCore.Annotations;
using SubscriptionAggregate = optiflow_platform.Subscription.Domain.Model.Aggregates.Subscription;

namespace optiflow_platform.Subscription.Interfaces.REST;

[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Checkout")]
[Authorize]
[AllowWithoutSubscription]
public class CheckoutController(
    IStripeCheckoutService stripeCheckoutService,
    ISubscriptionCommandService subscriptionCommandService,
    ISubscriptionQueryService subscriptionQueryService,
    ISubscriptionRepository subscriptionRepository,
    IPlanQueryService planQueryService,
    ICurrentUserContext currentUserContext,
    IConfiguration configuration,
    IWebHostEnvironment environment,
    ILogger<CheckoutController> logger) : ControllerBase
{
    /// <summary>
    ///     Starts checkout for the selected plan and returns a URL the frontend redirects to. When a
    ///     real Stripe key is configured it returns a hosted Stripe Checkout URL; otherwise (local dev)
    ///     it activates the subscription immediately and returns the frontend success URL.
    /// </summary>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Create checkout session",
        Description = "Creates a checkout session (real Stripe or dev-activate) and returns the redirect URL.",
        OperationId = "CreateCheckoutSession")]
    [SwaggerResponse(200, "Checkout session created", typeof(CheckoutSessionResource))]
    [SwaggerResponse(500, "Failed to create checkout session")]
    public async Task<ActionResult<CheckoutSessionResource>> CreateCheckoutSession(
        [FromBody] CreateCheckoutSessionResource resource, CancellationToken cancellationToken)
    {
        try
        {
            // Already active on this exact plan → nothing to charge; idempotent success. A different
            // plan means an upgrade/downgrade, which must be paid for like any other checkout.
            var existing = await subscriptionQueryService.GetCurrentActiveSubscriptionAsync(cancellationToken);
            if (existing != null && existing.PlanId.Value == resource.PlanId)
                return Ok(new CheckoutSessionResource(SuccessUrl()));

            var plan = await planQueryService.Handle(new GetPlanByIdQuery(new PlanId(resource.PlanId)), cancellationToken);
            if (plan is null)
                return Problem(title: "Checkout error", detail: "Selected plan does not exist.", statusCode: 400);

            // Outside local development, a real Stripe key is mandatory: never hand out access without a
            // real payment. Dev-activate (instant unlock, no charge) is a Development-only convenience.
            if (!UseRealStripeCheckout() && !environment.IsDevelopment())
                return Problem(title: "Checkout unavailable",
                    detail: "Online payment is not configured. Please contact support.", statusCode: 503);

            // Create the subscription in PENDING_PAYMENT for the current account.
            var selectCommand = new SelectSubscriptionPlanCommand(
                resource.AdminId, plan.PlanId, plan.Tier, resource.Amount, "CARD");
            var created = await subscriptionCommandService.Handle(selectCommand, cancellationToken);
            if (created is not Result<SubscriptionAggregate, Application.Errors.SelectSubscriptionPlanError>.Success createdOk)
                return Problem(title: "Checkout error", detail: "Could not create subscription.", statusCode: 500);
            var subscription = createdOk.Value;

            if (UseRealStripeCheckout())
            {
                // Stripe returns the browser to this backend endpoint, which activates the subscription
                // and redirects into the app; the webhook is a backup for the same activation.
                var returnUrl = $"{Request.Scheme}://{Request.Host}/api/v1/checkout/return";
                var url = stripeCheckoutService.CreateCheckoutSession(
                    subscription.Id, subscription.AccountId, resource.PlanName, resource.Amount, returnUrl);
                return Ok(new CheckoutSessionResource(url));
            }

            // Local dev without a real key: activate immediately.
            var now = DateTimeOffset.UtcNow;
            var activateCommand = new ActivateSubscriptionCommand(
                new SubscriptionId(subscription.Id), plan.Tier, now, now.AddYears(1));
            var activated = await subscriptionCommandService.Handle(activateCommand, cancellationToken);
            if (activated is not Result<SubscriptionAggregate, Application.Errors.ActivateSubscriptionError>.Success)
                return Problem(title: "Checkout error", detail: "Could not activate subscription.", statusCode: 500);

            // Upgrades: the just-activated plan replaces whatever was active before.
            await SupersedeOtherActiveSubscriptionsAsync(subscription.Id, cancellationToken);

            logger.LogInformation("Dev-activated subscription {SubscriptionId} for account {AccountId}",
                subscription.Id, currentUserContext.AccountId);
            return Ok(new CheckoutSessionResource(SuccessUrl()));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error creating checkout session for plan {PlanId}", resource.PlanId);
            return Problem(title: "Checkout error", detail: "Failed to create checkout session.", statusCode: 500);
        }
    }

    /// <summary>
    ///     Confirms a completed Stripe checkout by verifying the session was paid, then activates the
    ///     subscription. Lets payment grant access even when the webhook isn't reachable (local dev).
    /// </summary>
    [HttpPost("confirm")]
    [SwaggerOperation(Summary = "Confirm checkout payment", OperationId = "ConfirmCheckout")]
    [SwaggerResponse(200, "Confirmation result")]
    public async Task<ActionResult> ConfirmCheckout([FromQuery] string? sessionId, CancellationToken cancellationToken)
    {
        // No session to verify → just report whether something is already active (dev-activate or the
        // webhook/return already fired).
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            var current = await subscriptionQueryService.GetCurrentActiveSubscriptionAsync(cancellationToken);
            return Ok(new { active = current != null });
        }

        // A session id was supplied: always verify and activate that specific subscription, so an upgrade
        // (paid while another plan is still active) actually switches instead of reporting the old one.
        try
        {
            var session = new global::Stripe.Checkout.SessionService().Get(sessionId);
            var paid = session.PaymentStatus == "paid" || session.Status == "complete";
            if (paid
                && session.Metadata is not null
                && session.Metadata.TryGetValue("subscriptionId", out var subIdRaw)
                && int.TryParse(subIdRaw, out var subscriptionId)
                && session.Metadata.TryGetValue("accountId", out var accIdRaw)
                && Guid.TryParse(accIdRaw, out var accountId)
                && accountId == currentUserContext.AccountId)
            {
                var subscription = await subscriptionRepository.FindByIdAsync(subscriptionId, cancellationToken);
                if (subscription is not null)
                {
                    var now = DateTimeOffset.UtcNow;
                    await subscriptionCommandService.Handle(
                        new ActivateSubscriptionCommand(new SubscriptionId(subscriptionId), subscription.Tier, now, now.AddYears(1)),
                        cancellationToken);
                    await SupersedeOtherActiveSubscriptionsAsync(subscriptionId, cancellationToken);
                    logger.LogInformation("Confirmed and activated subscription {SubscriptionId} from Stripe session", subscriptionId);
                    return Ok(new { active = true });
                }
            }

            // Session not paid/attributable → fall back to whatever is currently active.
            var active = await subscriptionQueryService.GetCurrentActiveSubscriptionAsync(cancellationToken);
            return Ok(new { active = active != null });
        }
        catch (global::Stripe.StripeException ex)
        {
            logger.LogWarning(ex, "Failed to confirm Stripe session {SessionId}", sessionId);
            return Ok(new { active = false });
        }
    }

    /// <summary>
    ///     Cancels every other active subscription for the current account so the just-activated one is
    ///     the single source of truth after an upgrade or plan change. No-op on a first-time purchase.
    /// </summary>
    private async Task SupersedeOtherActiveSubscriptionsAsync(int keepSubscriptionId, CancellationToken cancellationToken)
    {
        var actives = await subscriptionRepository.FindByStatusAsync(SubscriptionStatus.Active, cancellationToken);
        foreach (var other in actives.Where(s => s.Id != keepSubscriptionId))
            await subscriptionCommandService.Handle(
                new CancelSubscriptionCommand(new SubscriptionId(other.Id)), cancellationToken);
    }

    private bool UseRealStripeCheckout()
    {
        var key = configuration["Stripe:SecretKey"];
        return !string.IsNullOrWhiteSpace(key) && key.StartsWith("sk_");
    }

    private string SuccessUrl()
    {
        // Relative path resolves against the frontend's own origin, so dev-activate works on any port.
        var configured = configuration["Stripe:SuccessUrl"];
        return !string.IsNullOrWhiteSpace(configured) ? configured : "/panel";
    }
}
