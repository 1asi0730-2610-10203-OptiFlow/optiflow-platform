namespace optiflow_platform.Subscription.Domain.Model.Queries;

/// <summary>
/// Issued to retrieve all payments associated with a given subscription.
/// </summary>
/// <param name="SubscriptionId"></param>
public record GetPaymentsBySubscriptionIdQuery(int SubscriptionId);
