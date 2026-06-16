using optiflow_platform.Inventory.Domain.Model.Aggregates;
using optiflow_platform.Shared.Domain.Repositories;

namespace optiflow_platform.Inventory.Domain.Repositories;

/// <summary>
///     Repository contract for <see cref="Supplier"/> persistence.
/// </summary>
public interface ISupplierRepository : IBaseRepository<Supplier>
{
    /// <summary>
    ///     Finds a supplier by its exact name, case-sensitively.
    /// </summary>
    Task<Supplier?> FindByNameAsync(string name, CancellationToken cancellationToken = default);
}
