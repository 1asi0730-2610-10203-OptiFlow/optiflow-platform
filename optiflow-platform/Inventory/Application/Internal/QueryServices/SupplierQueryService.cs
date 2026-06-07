using optiflow_platform.Inventory.Application.Services;
using optiflow_platform.Inventory.Domain.Model.Entities;
using optiflow_platform.Inventory.Domain.Model.Queries;
using optiflow_platform.Inventory.Domain.Repositories;

namespace optiflow_platform.Inventory.Application.Internal.QueryServices;

/// <summary>
///     Application service for handling supplier queries.
/// </summary>
/// <param name="supplierRepository">Repository for accessing supplier data.</param>
public class SupplierQueryService(ISupplierRepository supplierRepository) : ISupplierQueryService
{
    /// <inheritdoc />
    public async Task<IEnumerable<Supplier>> Handle(GetAllSuppliersQuery query,
        CancellationToken cancellationToken = default) =>
        await supplierRepository.ListAsync(cancellationToken);
}
