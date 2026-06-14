using optiflow_platform.Shared.Application.Internal.EventHandlers;
using optiflow_platform.Subscription.Domain.Model.Events;

namespace optiflow_platform.Subscription.Application.Internal.EventHandlers;

public class PaymentProcessingStartedEventHandler : IEventHandler<PaymentProcessingStartedEvent>
{
    public Task Handle(PaymentProcessingStartedEvent domainEvent, CancellationToken cancellationToken)
    {
        return On(domainEvent);
    }

    private static Task On(PaymentProcessingStartedEvent domainEvent)
    {
        Console.WriteLine("Payment processing started - SubscriptionId: " + domainEvent.SubscriptionId);
        return Task.CompletedTask;
    }
}
