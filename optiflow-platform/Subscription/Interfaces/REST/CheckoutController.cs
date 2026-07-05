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
    IPlanQueryService planQueryService,
    ICurrentUserContext currentUserContext,
    IConfiguration configuration,
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
            if (UseRealStripeCheckout())
            {
                var url = stripeCheckoutService.CreateCheckoutSession(
                    resource.AdminId, resource.PlanId, resource.PlanName, resource.Amount);
                return Ok(new CheckoutSessionResource(url));
            }

            return await DevActivateAsync(resource, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error creating checkout session for plan {PlanId}", resource.PlanId);
            return Problem(title: "Checkout error", detail: "Failed to create checkout session.", statusCode: 500);
        }
    }

    private bool UseRealStripeCheckout()
    {
        var key = configuration["Stripe:SecretKey"];
        return !string.IsNullOrWhiteSpace(key) && key.StartsWith("sk_");
    }

    private async Task<ActionResult<CheckoutSessionResource>> DevActivateAsync(
        CreateCheckoutSessionResource resource, CancellationToken cancellationToken)
    {
        // Already subscribed → nothing to charge, just send them to the success page.
        var existing = await subscriptionQueryService.GetCurrentActiveSubscriptionAsync(cancellationToken);
        if (existing != null)
            return Ok(new CheckoutSessionResource(SuccessUrl()));

        var plan = await planQueryService.Handle(new GetPlanByIdQuery(new PlanId(resource.PlanId)), cancellationToken);
        if (plan is null)
            return Problem(title: "Checkout error", detail: "Selected plan does not exist.", statusCode: 400);

        var selectCommand = new SelectSubscriptionPlanCommand(
            resource.AdminId, plan.PlanId, plan.Tier, resource.Amount, "CARD");
        var created = await subscriptionCommandService.Handle(selectCommand, cancellationToken);
        if (created is not Result<SubscriptionAggregate, Application.Errors.SelectSubscriptionPlanError>.Success createdOk)
            return Problem(title: "Checkout error", detail: "Could not create subscription.", statusCode: 500);

        var now = DateTimeOffset.UtcNow;
        var activateCommand = new ActivateSubscriptionCommand(
            new SubscriptionId(createdOk.Value.Id), plan.Tier, now, now.AddYears(1));
        var activated = await subscriptionCommandService.Handle(activateCommand, cancellationToken);
        if (activated is not Result<SubscriptionAggregate, Application.Errors.ActivateSubscriptionError>.Success)
            return Problem(title: "Checkout error", detail: "Could not activate subscription.", statusCode: 500);

        logger.LogInformation("Dev-activated subscription for account {AccountId} plan {PlanId}",
            currentUserContext.AccountId, resource.PlanId);
        return Ok(new CheckoutSessionResource(SuccessUrl()));
    }

    private string SuccessUrl()
    {
        // Relative path resolves against the frontend's own origin, so dev-activate works on any port.
        var configured = configuration["Stripe:SuccessUrl"];
        return !string.IsNullOrWhiteSpace(configured) ? configured : "/panel";
    }
}
