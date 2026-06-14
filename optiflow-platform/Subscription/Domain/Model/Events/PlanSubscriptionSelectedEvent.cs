using optiflow_platform.Shared.Domain.Model.Events;

namespace optiflow_platform.Subscription.Domain.Model.Events;

public record PlanSubscriptionSelectedEvent(int SubscriptionId, int AdminId, int PlanId) : IEvent;
