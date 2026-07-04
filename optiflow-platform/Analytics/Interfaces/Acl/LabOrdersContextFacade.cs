using optiflow_platform.LabAndOrders.Application.Services;
using optiflow_platform.LabAndOrders.Domain.Model.Queries;

namespace optiflow_platform.Analytics.Interfaces.Acl;

/// <summary>
///     Implements <see cref="ILabOrdersContextFacade"/> by delegating to LabAndOrders query services.
/// </summary>
/// <remarks>
///     Analytics never imports LabAndOrders domain types directly — only this facade does.
/// </remarks>
public class LabOrdersContextFacade(IWorkOrderQueryService workOrderQueryService) : ILabOrdersContextFacade
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<WorkOrderSummary>> FetchAllWorkOrdersAsync(
        CancellationToken cancellationToken = default)
    {
        var workOrders = await workOrderQueryService.Handle(new GetAllWorkOrdersQuery(), cancellationToken);
        return workOrders
            .Select(w => new WorkOrderSummary(w.SaleId, w.IsRework))
            .ToList();
    }
}
