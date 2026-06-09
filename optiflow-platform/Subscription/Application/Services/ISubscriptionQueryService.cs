using optiflow_platform.Subscription.Domain.Model.Aggregates;
using optiflow_platform.Subscription.Domain.Model.Queries;

namespace optiflow_platform.Subscription.Application.Services;

/// <summary>Contract for subscription query operations.</summary>
public interface ISubscriptionQueryService
{
    /// <summary>Handles retrieving a subscription by its identifier.</summary>
    Task<Subscription?> Handle(GetSubscriptionByIdQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>Handles retrieving all subscriptions.</summary>
    Task<IEnumerable<Subscription>> Handle(GetAllSubscriptionsQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>Handles retrieving all subscriptions belonging to a given admin.</summary>
    Task<IEnumerable<Subscription>> Handle(GetSubscriptionsByAdminIdQuery query,
        CancellationToken cancellationToken = default);
}
