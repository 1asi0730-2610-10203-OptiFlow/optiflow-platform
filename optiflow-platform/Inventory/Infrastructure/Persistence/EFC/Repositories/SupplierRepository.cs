using optiflow_platform.Inventory.Domain.Model.Entities;
using optiflow_platform.Inventory.Domain.Repositories;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace optiflow_platform.Inventory.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     Entity Framework repository for supplier persistence.
/// </summary>
/// <param name="context">The EF Core database context.</param>
public class SupplierRepository(AppDbContext context)
    : BaseRepository<Supplier>(context), ISupplierRepository
{
}
