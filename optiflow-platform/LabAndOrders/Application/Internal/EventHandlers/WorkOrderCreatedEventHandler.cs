using optiflow_platform.LabAndOrders.Domain.Model.Events;
using optiflow_platform.LabAndOrders.Interfaces.Acl;
using optiflow_platform.Shared.Application.Internal.EventHandlers;

namespace optiflow_platform.LabAndOrders.Application.Internal.EventHandlers;

/// <summary>
///     Handles the <see cref="WorkOrderCreatedEvent"/> domain event.
/// </summary>
/// <remarks>
///     Consumes the lens and frame material stock reserved for the work order in the Inventory context,
///     via the <see cref="IInventoryContextFacade"/> Anti-Corruption Layer.
/// </remarks>
public class WorkOrderCreatedEventHandler(
    IInventoryContextFacade inventoryContextFacade,
    ILogger<WorkOrderCreatedEventHandler> logger)
    : IEventHandler<WorkOrderCreatedEvent>
{
    /// <summary>
    ///     A work order carries a single lens type and a single frame selection,
    ///     so it consumes one unit of each material.
    /// </summary>
    private const int MaterialConsumptionQuantity = 1;

    /// <inheritdoc />
    public Task Handle(WorkOrderCreatedEvent domainEvent, CancellationToken cancellationToken)
    {
        return On(domainEvent, cancellationToken);
    }

    private async Task On(WorkOrderCreatedEvent domainEvent, CancellationToken cancellationToken)
    {
        if (domainEvent.LensProductId is { } lensProductId)
            await ConsumeMaterial(lensProductId, "Lente", domainEvent, cancellationToken);

        if (domainEvent.FrameProductId is { } frameProductId)
            await ConsumeMaterial(frameProductId, "Armazón", domainEvent, cancellationToken);
    }

    private async Task ConsumeMaterial(int productId, string materialLabel, WorkOrderCreatedEvent domainEvent,
        CancellationToken cancellationToken)
    {
        var consumed = await inventoryContextFacade.ConsumeStockAsync(
            productId, MaterialConsumptionQuantity,
            $"Orden de Trabajo #{domainEvent.WorkOrderId} ({materialLabel})", cancellationToken);

        if (!consumed)
            logger.LogWarning(
                "Could not consume {MaterialLabel} stock for product {ProductId} on work order {WorkOrderId} " +
                "(patient {PatientName}): product not found or insufficient stock",
                materialLabel, productId, domainEvent.WorkOrderId, domainEvent.PatientName);
    }
}
