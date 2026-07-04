using optiflow_platform.LabAndOrders.Application.Services;
using optiflow_platform.LabAndOrders.Domain.Model.Queries;

namespace optiflow_platform.Sales.Interfaces.Acl;

/// <summary>
///     Implements <see cref="ILabAndOrdersContextFacade"/> by delegating to LabAndOrders query services.
/// </summary>
/// <remarks>
///     Sales never imports LabAndOrders domain types directly — only this facade does.
///     If LabAndOrders internals change, only this class needs updating.
/// </remarks>
public class LabAndOrdersContextFacade(IWorkOrderQueryService workOrderQueryService)
    : ILabAndOrdersContextFacade
{
    /// <inheritdoc />
    public async Task<string?> FetchWorkOrderStatusBySaleId(int saleId,
        CancellationToken cancellationToken = default)
    {
        var workOrder = await workOrderQueryService.Handle(
            new GetWorkOrderBySaleIdQuery(saleId), cancellationToken);

        return workOrder?.Status;
    }

    /// <inheritdoc />
    public async Task<IReadOnlySet<int>> FetchWorkOrderMaterialProductIdsBySaleId(int saleId,
        CancellationToken cancellationToken = default)
    {
        var workOrder = await workOrderQueryService.Handle(
            new GetWorkOrderBySaleIdQuery(saleId), cancellationToken);

        if (workOrder is null) return new HashSet<int>();

        var productIds = new HashSet<int>();
        if (workOrder.LensProductId is { } lensProductId) productIds.Add(lensProductId);
        if (workOrder.FrameProductId is { } frameProductId) productIds.Add(frameProductId);
        return productIds;
    }
}
