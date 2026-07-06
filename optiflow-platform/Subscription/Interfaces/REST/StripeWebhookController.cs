using System.IO;
using Microsoft.AspNetCore.Mvc;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using optiflow_platform.Subscription.Application.Services;
using optiflow_platform.Subscription.Domain.Model.Commands;
using optiflow_platform.Subscription.Domain.Model.ValueObjects;
using optiflow_platform.Subscription.Domain.Repositories;
using Stripe;
using Stripe.Checkout;

namespace optiflow_platform.Subscription.Interfaces.REST;

/// <summary>
///     Receives Stripe webhook events. Public (Stripe calls it with no JWT); authenticity is proven by
///     the Stripe-Signature header verified against the configured webhook secret.
/// </summary>
[ApiController]
[Route("api/v1/checkout")]
public class StripeWebhookController(
    AppDbContext dbContext,
    ISubscriptionCommandService subscriptionCommandService,
    ISubscriptionRepository subscriptionRepository,
    IConfiguration configuration,
    ILogger<StripeWebhookController> logger) : ControllerBase
{
    [HttpPost("webhook")]
    public async Task<IActionResult> Handle(CancellationToken cancellationToken)
    {
        var payload = await new StreamReader(Request.Body).ReadToEndAsync(cancellationToken);
        var signature = Request.Headers["Stripe-Signature"];
        var webhookSecret = configuration["Stripe:WebhookSecret"];

        if (string.IsNullOrWhiteSpace(webhookSecret))
        {
            logger.LogWarning("Stripe webhook received but no webhook secret is configured");
            return BadRequest();
        }

        Event stripeEvent;
        try
        {
            stripeEvent = EventUtility.ConstructEvent(payload, signature, webhookSecret);
        }
        catch (StripeException ex)
        {
            logger.LogWarning(ex, "Rejected Stripe webhook with an invalid signature");
            return BadRequest();
        }

        if (stripeEvent.Type == "checkout.session.completed" && stripeEvent.Data.Object is Session session)
            await ActivateFromSessionAsync(session, cancellationToken);

        return Ok();
    }

    /// <summary>
    ///     Where Stripe sends the browser after a successful payment. Verifies the session, activates the
    ///     subscription server-side (no JWT needed — Stripe supplies the session id), then redirects into
    ///     the app. Public because it's a top-level browser navigation without an Authorization header.
    /// </summary>
    [HttpGet("return")]
    public async Task<IActionResult> Return([FromQuery(Name = "session_id")] string? sessionId, CancellationToken cancellationToken)
    {
        var frontend = configuration["AppSettings:FrontendUrl"]?.TrimEnd('/') ?? "http://localhost:5173";
        if (!string.IsNullOrWhiteSpace(sessionId))
        {
            try
            {
                var session = await new SessionService().GetAsync(sessionId, cancellationToken: cancellationToken);
                if (session.PaymentStatus == "paid" || session.Status == "complete")
                    await ActivateFromSessionAsync(session, cancellationToken);
            }
            catch (StripeException ex)
            {
                logger.LogWarning(ex, "Failed to verify Stripe session {SessionId} on return", sessionId);
            }
        }
        return Redirect($"{frontend}/payment-success?status=success");
    }

    private async Task ActivateFromSessionAsync(Session session, CancellationToken cancellationToken)
    {
        if (session.Metadata is null ||
            !session.Metadata.TryGetValue("subscriptionId", out var subscriptionIdRaw) ||
            !session.Metadata.TryGetValue("accountId", out var accountIdRaw) ||
            !int.TryParse(subscriptionIdRaw, out var subscriptionId) ||
            !Guid.TryParse(accountIdRaw, out var accountId))
            return;

        // No JWT here, so scope the account explicitly for the tenant query filter.
        dbContext.CurrentAccountId = accountId;
        var subscription = await subscriptionRepository.FindByIdAsync(subscriptionId, cancellationToken);
        if (subscription is null) return;

        var now = DateTimeOffset.UtcNow;
        await subscriptionCommandService.Handle(
            new ActivateSubscriptionCommand(new SubscriptionId(subscriptionId), subscription.Tier, now, now.AddYears(1)),
            cancellationToken);

        // Upgrade path: the newly paid subscription supersedes any previously active one for the account,
        // leaving exactly one active subscription.
        var actives = await subscriptionRepository.FindByStatusAsync(SubscriptionStatus.Active, cancellationToken);
        foreach (var other in actives.Where(s => s.Id != subscriptionId))
            await subscriptionCommandService.Handle(
                new CancelSubscriptionCommand(new SubscriptionId(other.Id)), cancellationToken);

        logger.LogInformation("Activated subscription {SubscriptionId} from Stripe", subscriptionId);
    }
}
