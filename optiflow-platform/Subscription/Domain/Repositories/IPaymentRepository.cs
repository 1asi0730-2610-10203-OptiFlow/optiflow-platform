using optiflow_platform.Shared.Domain.Repositories;
using optiflow_platform.Subscription.Domain.Model.Aggregates;

namespace optiflow_platform.Subscription.Domain.Repositories;

/// <summary>
///     Repository contract for subscription payment persistence.
/// </summary>
public interface IPaymentRepository : IBaseRepository<Payment>
{
    Task<IEnumerable<Payment>> FindBySubscriptionIdAsync(int subscriptionId,
        CancellationToken cancellationToken = default);
}
