using optiflow_platform.LabAndOrders.Domain.Model.Aggregates;
using optiflow_platform.Shared.Domain.Repositories;

namespace optiflow_platform.LabAndOrders.Domain.Repositories;

/// <summary>
///     Repository contract for laboratory persistence.
/// </summary>
public interface ILaboratoryRepository : IBaseRepository<Laboratory>
{
    /// <summary>
    ///     Finds a laboratory by its exact name, case-sensitively.
    /// </summary>
    Task<Laboratory?> FindByNameAsync(string name, CancellationToken cancellationToken = default);
}
