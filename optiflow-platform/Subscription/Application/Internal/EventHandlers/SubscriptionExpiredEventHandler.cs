using optiflow_platform.Shared.Application.Internal.EventHandlers;
using optiflow_platform.Subscription.Domain.Model.Events;

namespace optiflow_platform.Subscription.Application.Internal.EventHandlers;

public class SubscriptionExpiredEventHandler : IEventHandler<SubscriptionExpiredEvent>
{
    public Task Handle(SubscriptionExpiredEvent domainEvent, CancellationToken cancellationToken)
    {
        return On(domainEvent);
    }

    private static Task On(SubscriptionExpiredEvent domainEvent)
    {
        Console.WriteLine("Subscription expired - SubscriptionId: " + domainEvent.SubscriptionId + " - module access should be revoked");
        return Task.CompletedTask;
    }
}
