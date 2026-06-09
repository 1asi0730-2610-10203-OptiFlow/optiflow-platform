using optiflow_platform.Shared.Domain.Repositories;
using optiflow_platform.Subscription.Domain.Model.Aggregates;
using optiflow_platform.Subscription.Domain.Model.ValueObjects;

namespace optiflow_platform.Subscription.Domain.Repositories;

/// <summary>
///     Repository contract for subscription payment persistence.
/// </summary>
public interface IPaymentRepository : IBaseRepository<Payment>
{
    Task<IEnumerable<Payment>> FindBySubscriptionIdAsync(SubscriptionId subscriptionId,
        CancellationToken cancellationToken = default);
}
