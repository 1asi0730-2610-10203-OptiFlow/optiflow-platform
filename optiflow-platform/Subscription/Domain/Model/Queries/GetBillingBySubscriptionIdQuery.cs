using optiflow_platform.Subscription.Domain.Model.ValueObjects;

namespace optiflow_platform.Subscription.Domain.Model.Queries;

/// <summary>
/// Issued to retrieve the billing record associated with a given subscription.
/// </summary>
/// <param name="SubscriptionId"></param>
public record GetBillingBySubscriptionIdQuery(SubscriptionId SubscriptionId);
