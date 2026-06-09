namespace optiflow_platform.Subscription.Domain.Model.Commands;

/// <summary>
///     Issued when the billing system authorises an automatic renewal charge.
/// </summary>
/// <param name="SubscriptionId"></param>
public record RequestAutoRenewCommand(int SubscriptionId);