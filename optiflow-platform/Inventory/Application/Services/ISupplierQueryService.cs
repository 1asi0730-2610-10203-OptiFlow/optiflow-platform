using optiflow_platform.Inventory.Domain.Model.Entities;
using optiflow_platform.Inventory.Domain.Model.Queries;

namespace optiflow_platform.Inventory.Application.Services;

/// <summary>
///     Contract for supplier query operations.
/// </summary>
public interface ISupplierQueryService
{
    /// <summary>Returns all suppliers.</summary>
    Task<IEnumerable<Supplier>> Handle(GetAllSuppliersQuery query, CancellationToken cancellationToken = default);
}
