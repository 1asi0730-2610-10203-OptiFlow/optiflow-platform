using optiflow_platform.Inventory.Domain.Model.Aggregates;
using optiflow_platform.Inventory.Domain.Repositories;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace optiflow_platform.Inventory.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     Entity Framework repository for stock audit log persistence.
/// </summary>
/// <param name="context">The EF Core database context.</param>
public class StockAuditLogRepository(AppDbContext context)
    : BaseRepository<StockAuditLog>(context), IStockAuditLogRepository
{
}
