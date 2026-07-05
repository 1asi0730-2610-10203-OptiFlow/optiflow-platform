using optiflow_platform.Subscription.Domain.Model.Queries;
using SubscriptionAggregate = optiflow_platform.Subscription.Domain.Model.Aggregates.Subscription;

namespace optiflow_platform.Subscription.Application.Services;

/// <summary>Contract for subscription query operations.</summary>
public interface ISubscriptionQueryService
{
    /// <summary>Handles retrieving a subscription by its identifier.</summary>
    Task<SubscriptionAggregate?> Handle(GetSubscriptionByIdQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>Handles retrieving all subscriptions.</summary>
    Task<IEnumerable<SubscriptionAggregate>> Handle(GetAllSubscriptionsQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>Handles retrieving all subscriptions belonging to a given admin.</summary>
    Task<IEnumerable<SubscriptionAggregate>> Handle(GetSubscriptionsByAdminIdQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Returns the current account's active subscription, or null if the account has none.
    ///     Scoped to the caller's account by the global query filter.
    /// </summary>
    Task<SubscriptionAggregate?> GetCurrentActiveSubscriptionAsync(CancellationToken cancellationToken = default);
}
