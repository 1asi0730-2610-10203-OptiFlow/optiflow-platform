using optiflow_platform.Inventory.Application.Errors;
using optiflow_platform.Inventory.Application.Services;
using optiflow_platform.Inventory.Domain.Model.Aggregates;
using optiflow_platform.Inventory.Domain.Model.Commands;
using optiflow_platform.Sales.Domain.Model.Events;
using optiflow_platform.Shared.Application.Internal.EventHandlers;
using optiflow_platform.Shared.Application.Patterns;

namespace optiflow_platform.Inventory.Application.Internal.EventHandlers;

/// <summary>
///     Depletes catalog stock for every item sold when a sale completes.
/// </summary>
/// <remarks>
///     Runs in-process after the sale has already been committed as Paid (same
///     fire-and-forget convention every other event in this codebase follows —
///     see other handlers under Sales/Application/Internal/EventHandlers). If an
///     item can't be reduced (e.g. insufficient stock), it's logged and the rest
///     of the sale's items are still processed; the sale itself is not rolled back.
/// </remarks>
public class SaleCompletedEventHandler(
    IProductCommandService productCommandService,
    ILogger<SaleCompletedEventHandler> logger)
    : IEventHandler<SaleCompletedEvent>
{
    public async Task Handle(SaleCompletedEvent domainEvent, CancellationToken cancellationToken)
    {
        foreach (var item in domainEvent.Items)
        {
            var reason = $"Sale #{domainEvent.SaleId} ({domainEvent.UserName})";
            var result = await productCommandService.Handle(
                new ReduceStockCommand(item.ProductId, item.Quantity, reason), cancellationToken);

            if (result is Result<Product, ReduceStockError>.Failure failure)
                logger.LogError(
                    "Could not deplete stock for product {ProductId} (qty {Quantity}) from sale {SaleId}: {Error}",
                    item.ProductId, item.Quantity, domainEvent.SaleId, failure.Error);
        }
    }
}
