using optiflow_platform.Subscription.Application.Services;
using optiflow_platform.Subscription.Domain.Model.Aggregates;
using optiflow_platform.Subscription.Domain.Model.Queries;
using optiflow_platform.Subscription.Domain.Repositories;

namespace optiflow_platform.Subscription.Application.Internal.QueryServices;

/// <summary>Application service for handling subscription queries.</summary>
public class SubscriptionQueryService(ISubscriptionRepository subscriptionRepository)
    : ISubscriptionQueryService
{
    /// <inheritdoc />
    public async Task<Subscription?> Handle(GetSubscriptionByIdQuery query,
        CancellationToken cancellationToken = default) =>
        await subscriptionRepository.FindByIdAsync(query.Id.Value, cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<Subscription>> Handle(GetAllSubscriptionsQuery query,
        CancellationToken cancellationToken = default) =>
        await subscriptionRepository.ListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<Subscription>> Handle(GetSubscriptionsByAdminIdQuery query,
        CancellationToken cancellationToken = default) =>
        await subscriptionRepository.FindByAdminIdAsync(query.AdminId, cancellationToken);
}
