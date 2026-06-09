namespace optiflow_platform.Subscription.Domain.Model.Commands;

/// <summary>
/// Issued when Admin decides to active his subscription
/// </summary>
/// <param name="SubscriptionId"></param>
/// <param name="Tier"></param>
/// <param name="StartDate"></param>
/// <param name="EndDate"></param>
public record ActivateSubscriptionCommand(int SubscriptionId, string Tier, DateTimeOffset StartDate, DateTimeOffset EndDate);