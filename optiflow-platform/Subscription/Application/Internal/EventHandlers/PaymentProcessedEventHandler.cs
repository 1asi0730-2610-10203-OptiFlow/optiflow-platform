using Microsoft.Extensions.Configuration;
using optiflow_platform.Shared.Application.Internal.EventHandlers;
using optiflow_platform.Subscription.Application.Services;
using optiflow_platform.Subscription.Domain.Model.Commands;
using optiflow_platform.Subscription.Domain.Model.Events;
using optiflow_platform.Subscription.Domain.Model.Queries;
using optiflow_platform.Subscription.Domain.Model.ValueObjects;

namespace optiflow_platform.Subscription.Application.Internal.EventHandlers;

public class PaymentProcessedEventHandler(
    ISubscriptionQueryService subscriptionQueryService,
    ISubscriptionCommandService subscriptionCommandService,
    IConfiguration configuration)
    : IEventHandler<PaymentProcessedEvent>
{
    public Task Handle(PaymentProcessedEvent domainEvent, CancellationToken cancellationToken)
    {
        return On(domainEvent, cancellationToken);
    }

    private async Task On(PaymentProcessedEvent domainEvent, CancellationToken cancellationToken)
    {
        // With a real Stripe key, a subscription must only be activated once Stripe confirms the
        // payment (handled by the checkout return + webhook). Activating here — at plan selection,
        // before the user has paid — would grant access for free. The simulated dev-activate flow
        // (no real key) still activates immediately here.
        var stripeKey = configuration["Stripe:SecretKey"];
        if (!string.IsNullOrWhiteSpace(stripeKey) && stripeKey.StartsWith("sk_"))
            return;

        var subscription = await subscriptionQueryService.Handle(
            new GetSubscriptionByIdQuery(new SubscriptionId(domainEvent.SubscriptionId)), cancellationToken);

        if (subscription is null)
        {
            Console.WriteLine("Subscription not found for activation - SubscriptionId: " + domainEvent.SubscriptionId);
            return;
        }

        await subscriptionCommandService.Handle(
            new ActivateSubscriptionCommand(
                new SubscriptionId(domainEvent.SubscriptionId),
                subscription.Tier,
                DateTimeOffset.UtcNow,
                DateTimeOffset.UtcNow.AddYears(1)),
            cancellationToken);
    }
}
