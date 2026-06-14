using optiflow_platform.Shared.Domain.Model.Events;

namespace optiflow_platform.Subscription.Domain.Model.Events;

public record PaymentProcessedEvent(int PaymentId, int SubscriptionId, decimal Amount) : IEvent;
