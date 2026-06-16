using optiflow_platform.Shared.Domain.Model.Events;

namespace optiflow_platform.Subscription.Domain.Model.Events;

public record SubscriptionEndDateCheckedEvent(int BillingId, int SubscriptionId) : IEvent;
