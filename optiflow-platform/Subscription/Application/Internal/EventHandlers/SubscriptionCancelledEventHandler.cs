using optiflow_platform.Shared.Application.Internal.EventHandlers;
using optiflow_platform.Subscription.Domain.Model.Events;

namespace optiflow_platform.Subscription.Application.Internal.EventHandlers;

public class SubscriptionCancelledEventHandler : IEventHandler<SubscriptionCancelledEvent>
{
    public Task Handle(SubscriptionCancelledEvent domainEvent, CancellationToken cancellationToken)
    {
        return On(domainEvent);
    }

    private static Task On(SubscriptionCancelledEvent domainEvent)
    {
        Console.WriteLine("Subscription cancelled - SubscriptionId: " + domainEvent.SubscriptionId + " - module access should be revoked");
        return Task.CompletedTask;
    }
}
