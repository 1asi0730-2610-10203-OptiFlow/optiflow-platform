using optiflow_platform.Inventory.Domain.Model.Aggregates;
using optiflow_platform.Shared.Domain.Repositories;

namespace optiflow_platform.Inventory.Domain.Repositories;

/// <summary>
///     Repository contract for <see cref="StockAuditLog"/> persistence.
/// </summary>
public interface IStockAuditLogRepository : IBaseRepository<StockAuditLog>
{
}
