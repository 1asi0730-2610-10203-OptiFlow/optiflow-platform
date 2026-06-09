
namespace optiflow_platform.Subscription.Domain.Model.Commands;

/// <summary>
///     Issued by the system when a subscription's end date is reached or renewal fails.
/// </summary>
/// <param name="SubscriptionId"></param>
/// 
public record ExpireSubscriptionCommand(int SubscriptionId);