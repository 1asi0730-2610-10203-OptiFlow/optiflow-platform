using optiflow_platform.Shared.Application.Internal.EventHandlers;
using optiflow_platform.Subscription.Domain.Model.Events;

namespace optiflow_platform.Subscription.Application.Internal.EventHandlers;

public class SubscriptionEndDateCheckedEventHandler : IEventHandler<SubscriptionEndDateCheckedEvent>
{
    public Task Handle(SubscriptionEndDateCheckedEvent domainEvent, CancellationToken cancellationToken)
    {
        return On(domainEvent);
    }

    private static Task On(SubscriptionEndDateCheckedEvent domainEvent)
    {
        Console.WriteLine("Subscription end date checked - SubscriptionId: " + domainEvent.SubscriptionId + " - module access should be revoked if end date reached");
        return Task.CompletedTask;
    }
}
