namespace optiflow_platform.Subscription.Domain.Model.Queries;

/// <summary>
/// Issued to retrieve a subscription by its unique identifier.
/// </summary>
/// <param name="Id"></param>
public record GetSubscriptionByIdQuery(int Id);
