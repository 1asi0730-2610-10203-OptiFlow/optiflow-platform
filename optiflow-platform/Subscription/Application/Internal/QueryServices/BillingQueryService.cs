using optiflow_platform.Subscription.Application.Services;
using optiflow_platform.Subscription.Domain.Model.Aggregates;
using optiflow_platform.Subscription.Domain.Model.Queries;
using optiflow_platform.Subscription.Domain.Repositories;

namespace optiflow_platform.Subscription.Application.Internal.QueryServices;

/// <summary>Application service for handling billing queries.</summary>
public class BillingQueryService(IBillingRepository billingRepository)
    : IBillingQueryService
{
    /// <inheritdoc />
    public async Task<Billing?> Handle(GetBillingBySubscriptionIdQuery query,
        CancellationToken cancellationToken = default) =>
        await billingRepository.FindBySubscriptionIdAsync(query.SubscriptionId, cancellationToken);
}
