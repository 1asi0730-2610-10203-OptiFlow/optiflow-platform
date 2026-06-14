using optiflow_platform.Shared.Application.Internal.EventHandlers;
using optiflow_platform.Subscription.Application.Services;
using optiflow_platform.Subscription.Domain.Model.Commands;
using optiflow_platform.Subscription.Domain.Model.Events;
using optiflow_platform.Subscription.Domain.Model.Queries;
using optiflow_platform.Subscription.Domain.Model.ValueObjects;

namespace optiflow_platform.Subscription.Application.Internal.EventHandlers;

public class PaymentProcessedEventHandler(
    ISubscriptionQueryService subscriptionQueryService,
    ISubscriptionCommandService subscriptionCommandService)
    : IEventHandler<PaymentProcessedEvent>
{
    public Task Handle(PaymentProcessedEvent domainEvent, CancellationToken cancellationToken)
    {
        return On(domainEvent, cancellationToken);
    }

    private async Task On(PaymentProcessedEvent domainEvent, CancellationToken cancellationToken)
    {
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
