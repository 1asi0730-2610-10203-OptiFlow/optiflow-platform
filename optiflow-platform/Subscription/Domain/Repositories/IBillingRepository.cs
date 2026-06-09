using optiflow_platform.Shared.Domain.Repositories;
using optiflow_platform.Subscription.Domain.Model.Aggregates;
using optiflow_platform.Subscription.Domain.Model.ValueObjects;

namespace optiflow_platform.Subscription.Domain.Repositories;

/// <summary>
///     Repository contract for billing persistence.
/// </summary>
public interface IBillingRepository : IBaseRepository<Billing>
{
    Task<Billing?> FindBySubscriptionIdAsync(SubscriptionId subscriptionId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Billing>> FindDueForRenewalAsync(DateTimeOffset date,
        CancellationToken cancellationToken = default);
}
