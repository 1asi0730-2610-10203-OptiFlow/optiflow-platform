using optiflow_platform.Subscription.Domain.Model.ValueObjects;

namespace optiflow_platform.Subscription.Domain.Model.Commands;

/// <summary>
///     Issued by the system to check whether the subscription end date has been reached.
/// </summary>
/// <param name="SubscriptionId"></param>
public record CheckSubscriptionEndDateCommand(SubscriptionId SubscriptionId);
