using optiflow_platform.Shared.Application.Internal.EventHandlers;
using optiflow_platform.Subscription.Domain.Model.Events;

namespace optiflow_platform.Subscription.Application.Internal.EventHandlers;

public class RequestedRenewSubscriptionEventHandler : IEventHandler<RequestedRenewSubscriptionEvent>
{
    public Task Handle(RequestedRenewSubscriptionEvent domainEvent, CancellationToken cancellationToken)
    {
        return On(domainEvent);
    }

    private static Task On(RequestedRenewSubscriptionEvent domainEvent)
    {
        Console.WriteLine("Auto-renew requested - SubscriptionId: " + domainEvent.SubscriptionId + ", BillingId: " + domainEvent.BillingId);
        return Task.CompletedTask;
    }
}
