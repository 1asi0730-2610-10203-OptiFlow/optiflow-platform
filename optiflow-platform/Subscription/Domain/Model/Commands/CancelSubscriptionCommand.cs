using optiflow_platform.Subscription.Domain.Model.ValueObjects;

namespace optiflow_platform.Subscription.Domain.Model.Commands;

/// <summary>
///  Issued when an admin explicitly cancels the subscription.
/// </summary>
/// <param name="SubscriptionId"></param>
public record CancelSubscriptionCommand(SubscriptionId SubscriptionId);
