using optiflow_platform.LabAndOrders.Domain.Model.Aggregates;
using optiflow_platform.LabAndOrders.Domain.Model.Queries;

namespace optiflow_platform.LabAndOrders.Application.Services;

/// <summary>
///     Contract for work order query operations.
/// </summary>
public interface IWorkOrderQueryService
{
    /// <summary>
    ///     Handles retrieval of all work orders.
    /// </summary>
    Task<IEnumerable<WorkOrder>> Handle(GetAllWorkOrdersQuery query, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Handles retrieval of a work order by its identifier.
    /// </summary>
    Task<WorkOrder?> Handle(GetWorkOrderByIdQuery query, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Handles retrieval of the work order linked to a given sale.
    /// </summary>
    Task<WorkOrder?> Handle(GetWorkOrderBySaleIdQuery query, CancellationToken cancellationToken = default);
}
