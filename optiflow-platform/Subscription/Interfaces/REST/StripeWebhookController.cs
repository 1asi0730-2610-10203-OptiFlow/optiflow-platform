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

        if (stripeEvent.Type == "checkout.session.completed" &&
            stripeEvent.Data.Object is Session session &&
            session.Metadata is not null &&
            session.Metadata.TryGetValue("subscriptionId", out var subscriptionIdRaw) &&
            session.Metadata.TryGetValue("accountId", out var accountIdRaw) &&
            int.TryParse(subscriptionIdRaw, out var subscriptionId) &&
            Guid.TryParse(accountIdRaw, out var accountId))
        {
            // No JWT here, so scope the account explicitly for the tenant query filter.
            dbContext.CurrentAccountId = accountId;
            var subscription = await subscriptionRepository.FindByIdAsync(subscriptionId, cancellationToken);
            if (subscription is not null)
            {
                var now = DateTimeOffset.UtcNow;
                var command = new ActivateSubscriptionCommand(
                    new SubscriptionId(subscriptionId), subscription.Tier, now, now.AddYears(1));
                await subscriptionCommandService.Handle(command, cancellationToken);
                logger.LogInformation("Activated subscription {SubscriptionId} from Stripe webhook", subscriptionId);
            }
        }

        return Ok();
    }
}
