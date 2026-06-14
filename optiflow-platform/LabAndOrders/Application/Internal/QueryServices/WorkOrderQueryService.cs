using optiflow_platform.LabAndOrders.Application.Services;
using optiflow_platform.LabAndOrders.Domain.Model.Aggregates;
using optiflow_platform.LabAndOrders.Domain.Model.Queries;
using optiflow_platform.LabAndOrders.Domain.Repositories;

namespace optiflow_platform.LabAndOrders.Application.Internal.QueryServices;

/// <summary>
///     Application service for querying work orders.
/// </summary>
/// <param name="workOrderRepository">Repository for accessing work order data.</param>
public class WorkOrderQueryService(IWorkOrderRepository workOrderRepository) : IWorkOrderQueryService
{
    /// <inheritdoc />
    public async Task<IEnumerable<WorkOrder>> Handle(GetAllWorkOrdersQuery query,
        CancellationToken cancellationToken = default) =>
        await workOrderRepository.ListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<WorkOrder?> Handle(GetWorkOrderByIdQuery query,
        CancellationToken cancellationToken = default) =>
        await workOrderRepository.FindByIdAsync(query.Id, cancellationToken);

    /// <inheritdoc />
    public async Task<WorkOrder?> Handle(GetWorkOrderBySaleIdQuery query,
        CancellationToken cancellationToken = default) =>
        await workOrderRepository.FindBySaleIdAsync(query.SaleId, cancellationToken);
}
