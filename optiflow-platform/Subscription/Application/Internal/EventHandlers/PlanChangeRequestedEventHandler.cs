using optiflow_platform.Shared.Application.Internal.EventHandlers;
using optiflow_platform.Subscription.Domain.Model.Events;

namespace optiflow_platform.Subscription.Application.Internal.EventHandlers;

public class PlanChangeRequestedEventHandler : IEventHandler<PlanChangeRequestedEvent>
{
    public Task Handle(PlanChangeRequestedEvent domainEvent, CancellationToken cancellationToken)
    {
        return On(domainEvent);
    }

    private static Task On(PlanChangeRequestedEvent domainEvent)
    {
        Console.WriteLine("Plan change requested - SubscriptionId: " + domainEvent.SubscriptionId + ", NewPlanId: " + domainEvent.NewPlanId);
        return Task.CompletedTask;
    }
}
