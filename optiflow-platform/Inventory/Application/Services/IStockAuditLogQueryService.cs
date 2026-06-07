using optiflow_platform.Inventory.Domain.Model.Aggregates;
using optiflow_platform.Inventory.Domain.Model.Queries;

namespace optiflow_platform.Inventory.Application.Services;

/// <summary>
///     Contract for stock audit log query operations.
/// </summary>
public interface IStockAuditLogQueryService
{
    /// <summary>Returns the stock audit log entries, optionally filtered by a recorded date period.</summary>
    Task<IEnumerable<StockAuditLog>> Handle(GetAuditLogsQuery query, CancellationToken cancellationToken = default);
}
