using optiflow_platform.Sales.Domain.Model.Events;
using optiflow_platform.Sales.Domain.Repositories;
using optiflow_platform.Shared.Application.Errors;
using optiflow_platform.Shared.Application.Internal.EventHandlers;
using optiflow_platform.Shared.Application.Patterns;
using optiflow_platform.Shared.Application.Services;
using optiflow_platform.Shared.Domain.Model.Commands;
using optiflow_platform.Shared.Domain.Model.Entities;
using optiflow_platform.Shared.Domain.Model.ValueObjects;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace optiflow_platform.Sales.Application.Internal.EventHandlers;

/// <summary>
///     Cortex.Mediator invokes every <see cref="IEventHandler{TEvent}"/> for a given event
///     concurrently, and the Inventory stock-depletion handler for the same
///     <see cref="SaleCompletedEvent"/> runs in parallel with this one. EF Core's DbContext is
///     scoped per request and not safe for concurrent use, so this handler resolves its own
///     dependencies from a fresh DI scope instead of taking them via constructor injection.
/// </summary>
public class SaleCompletedEventHandler(
    IServiceScopeFactory scopeFactory,
    ILogger<SaleCompletedEventHandler> logger)
    : IEventHandler<SaleCompletedEvent>
{
    public Task Handle(SaleCompletedEvent domainEvent, CancellationToken cancellationToken)
    {
        return On(domainEvent, cancellationToken);
    }

    private async Task On(SaleCompletedEvent domainEvent, CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        // This fresh scope's AppDbContext starts with no account set — the request that
        // published this event already ended. Stamp it from the event before any query
        // runs, or the account query filters below would see everything as out of scope.
        scope.ServiceProvider.GetRequiredService<AppDbContext>().CurrentAccountId = domainEvent.AccountId;
        var saleRepository = scope.ServiceProvider.GetRequiredService<ISaleRepository>();
        var notificationCommandService = scope.ServiceProvider.GetRequiredService<ISystemNotificationCommandService>();

        var sale = await saleRepository.FindByIdAsync(domainEvent.SaleId.Value, cancellationToken);
        if (sale is null)
        {
            logger.LogWarning("Sale {SaleId} not found while notifying completion", domainEvent.SaleId);
            return;
        }

        var message = $"Sale #{sale.Id} for {sale.PatientName} has been completed and paid in full.";
        var command = new CreateSystemNotificationCommand(sale.UserId, NotificationCategory.SaleCompleted, message, sale.AccountId);
        var result  = await notificationCommandService.Handle(command, cancellationToken);

        if (result is Result<SystemNotification, CreateSystemNotificationError>.Failure failure)
            logger.LogError("Could not create sale completion notification for sale {SaleId}: {Error}",
                domainEvent.SaleId, failure.Error);
    }
}
