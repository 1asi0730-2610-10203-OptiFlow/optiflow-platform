using optiflow_platform.Shared.Domain.Repositories;
using optiflow_platform.Subscription.Domain.Model.Aggregates;

namespace optiflow_platform.Subscription.Domain.Repositories;

/// <summary>Repository contract for plan persistence.</summary>
public interface IPlanRepository : IBaseRepository<Plan>
{
    Task<Plan?> FindByNameAsync(string name, CancellationToken cancellationToken = default);
}
