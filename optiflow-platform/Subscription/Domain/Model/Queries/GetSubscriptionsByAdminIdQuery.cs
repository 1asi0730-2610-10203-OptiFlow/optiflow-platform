namespace optiflow_platform.Subscription.Domain.Model.Queries;

/// <summary>
/// Issued to retrieve all subscriptions belonging to a given admin.
/// </summary>
/// <param name="AdminId"></param>
public record GetSubscriptionsByAdminIdQuery(int AdminId);
