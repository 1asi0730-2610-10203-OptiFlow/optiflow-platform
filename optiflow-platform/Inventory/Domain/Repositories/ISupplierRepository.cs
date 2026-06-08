using optiflow_platform.Inventory.Domain.Model.Entities;
using optiflow_platform.Shared.Domain.Repositories;

namespace optiflow_platform.Inventory.Domain.Repositories;

/// <summary>
///     Repository contract for <see cref="Supplier"/> persistence.
/// </summary>
public interface ISupplierRepository : IBaseRepository<Supplier>
{
}
