using optiflow_platform.Inventory.Application.Errors;
using optiflow_platform.Inventory.Application.Services;
using optiflow_platform.Inventory.Domain.Model.Aggregates;
using optiflow_platform.Inventory.Domain.Model.Commands;
using optiflow_platform.LabAndOrders.Domain.Model.Events;
using optiflow_platform.Shared.Application.Internal.EventHandlers;
using optiflow_platform.Shared.Application.Patterns;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace optiflow_platform.Inventory.Application.Internal.EventHandlers;

/// <summary>
///     Depletes catalog stock for the lens and frame products consumed by a newly created work order.
/// </summary>
/// <remarks>
///     Runs in-process after the work order has already been committed (same fire-and-forget
///     convention as <see cref="SaleCompletedEventHandler"/>). If a product can't be reduced
///     (e.g. insufficient stock), it's logged and the work order itself is not rolled back.
/// </remarks>
/// <remarks>
///     Cortex.Mediator invokes every <see cref="IEventHandler{TEvent}"/> for a given event
///     concurrently. EF Core's DbContext is scoped per request and not safe for concurrent use,
///     so this handler resolves its own dependencies from a fresh DI scope instead of taking
///     them via constructor injection — same caveat as <see cref="SaleCompletedEventHandler"/>.
/// </remarks>
public class WorkOrderCreatedEventHandler(
    IServiceScopeFactory scopeFactory,
    ILogger<WorkOrderCreatedEventHandler> logger)
    : IEventHandler<WorkOrderCreatedEvent>
{
    public async Task Handle(WorkOrderCreatedEvent domainEvent, CancellationToken cancellationToken)
    {
        if (domainEvent.LensProductId is null && domainEvent.FrameProductId is null)
            return;

        using var scope = scopeFactory.CreateScope();
        scope.ServiceProvider.GetRequiredService<AppDbContext>().CurrentAccountId = domainEvent.AccountId;
        var productCommandService = scope.ServiceProvider.GetRequiredService<IProductCommandService>();
        var reason = $"Work order #{domainEvent.WorkOrderId} ({domainEvent.PatientName})";

        if (domainEvent.LensProductId is { } lensProductId)
            await ReduceOne(productCommandService, lensProductId, reason, domainEvent, logger, cancellationToken);

        if (domainEvent.FrameProductId is { } frameProductId)
            await ReduceOne(productCommandService, frameProductId, reason, domainEvent, logger, cancellationToken);
    }

    private static async Task ReduceOne(
        IProductCommandService productCommandService, int productId, string reason,
        WorkOrderCreatedEvent domainEvent, ILogger logger, CancellationToken cancellationToken)
    {
        var result = await productCommandService.Handle(
            new ReduceStockCommand(productId, 1, reason), cancellationToken);

        if (result is Result<Product, ReduceStockError>.Failure failure)
            logger.LogError(
                "Could not deplete stock for product {ProductId} from work order {WorkOrderId}: {Error}",
                productId, domainEvent.WorkOrderId, failure.Error);
    }
}
