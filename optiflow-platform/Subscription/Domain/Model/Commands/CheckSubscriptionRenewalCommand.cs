using optiflow_platform.Subscription.Domain.Model.ValueObjects;

namespace optiflow_platform.Subscription.Domain.Model.Commands;

/// <summary>
///     Issued by the system (scheduler) to evaluate whether a subscription is due for renewal.
/// </summary>
/// <param name="SubscriptionId"></param>
public record CheckSubscriptionRenewalCommand(SubscriptionId SubscriptionId);
