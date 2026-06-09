using optiflow_platform.Shared.Domain.Repositories;
using optiflow_platform.Subscription.Domain.Model.Aggregates;

namespace optiflow_platform.Subscription.Domain.Repositories;

/// <summary>
///     Repository contract for subscription persistence.
/// </summary>
public interface ISubscriptionRepository : IBaseRepository<Subscription>
{
    Task<IEnumerable<Subscription>> FindByAdminIdAsync(int adminId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Subscription>> FindByStatusAsync(string status,
        CancellationToken cancellationToken = default);
}
