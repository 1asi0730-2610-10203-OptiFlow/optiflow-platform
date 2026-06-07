using optiflow_platform.Inventory.Application.Services;
using optiflow_platform.Inventory.Domain.Model.Aggregates;
using optiflow_platform.Inventory.Domain.Model.Queries;
using optiflow_platform.Inventory.Domain.Repositories;

namespace optiflow_platform.Inventory.Application.Internal.QueryServices;

/// <summary>
///     Application service for handling stock audit log queries.
/// </summary>
/// <param name="stockAuditLogRepository">Repository for accessing stock audit log data.</param>
public class StockAuditLogQueryService(IStockAuditLogRepository stockAuditLogRepository) : IStockAuditLogQueryService
{
    /// <inheritdoc />
    public async Task<IEnumerable<StockAuditLog>> Handle(GetAuditLogsQuery query,
        CancellationToken cancellationToken = default)
    {
        var logs = await stockAuditLogRepository.ListAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(query.From) || string.IsNullOrWhiteSpace(query.To))
            return logs;

        return logs.Where(log =>
            string.CompareOrdinal(log.Date, query.From) >= 0 &&
            string.CompareOrdinal(log.Date, query.To) <= 0);
    }
}
