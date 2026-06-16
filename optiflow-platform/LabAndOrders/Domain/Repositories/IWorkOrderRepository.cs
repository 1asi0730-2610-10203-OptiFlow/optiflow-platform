using optiflow_platform.LabAndOrders.Domain.Model.Aggregates;
using optiflow_platform.Shared.Domain.Repositories;

namespace optiflow_platform.LabAndOrders.Domain.Repositories;

/// <summary>
///     Repository contract for work order persistence.
/// </summary>
public interface IWorkOrderRepository : IBaseRepository<WorkOrder>
{
    /// <summary>
    ///     Find all work orders with a given status.
    /// </summary>
    /// <param name="status">The order status to filter by.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>An enumerable of work orders matching the given status.</returns>
    Task<IEnumerable<WorkOrder>> FindByStatusAsync(string status, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Find the work order associated with a given sale.
    /// </summary>
    Task<WorkOrder?> FindBySaleIdAsync(int saleId, CancellationToken cancellationToken = default);
}
