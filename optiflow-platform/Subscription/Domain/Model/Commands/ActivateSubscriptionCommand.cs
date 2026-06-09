using optiflow_platform.Subscription.Domain.Model.ValueObjects;

namespace optiflow_platform.Subscription.Domain.Model.Commands;

/// <summary>
/// Issued when Admin decides to active his subscription
/// </summary>
/// <param name="SubscriptionId"></param>
/// <param name="Tier"></param>
/// <param name="StartDate"></param>
/// <param name="EndDate"></param>
public record ActivateSubscriptionCommand(SubscriptionId SubscriptionId, SubscriptionTier Tier, DateTimeOffset StartDate, DateTimeOffset EndDate);
