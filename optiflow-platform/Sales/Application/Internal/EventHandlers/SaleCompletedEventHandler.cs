using optiflow_platform.Sales.Domain.Model.Events;
using optiflow_platform.Sales.Domain.Repositories;
using optiflow_platform.Shared.Application.Errors;
using optiflow_platform.Shared.Application.Internal.EventHandlers;
using optiflow_platform.Shared.Application.Patterns;
using optiflow_platform.Shared.Application.Services;
using optiflow_platform.Shared.Domain.Model.Commands;
using optiflow_platform.Shared.Domain.Model.Entities;
using optiflow_platform.Shared.Domain.Model.ValueObjects;

namespace optiflow_platform.Sales.Application.Internal.EventHandlers;

public class SaleCompletedEventHandler(
    ISaleRepository saleRepository,
    ISystemNotificationCommandService notificationCommandService,
    ILogger<SaleCompletedEventHandler> logger)
    : IEventHandler<SaleCompletedEvent>
{
    public Task Handle(SaleCompletedEvent domainEvent, CancellationToken cancellationToken)
    {
        return On(domainEvent, cancellationToken);
    }

    private async Task On(SaleCompletedEvent domainEvent, CancellationToken cancellationToken)
    {
        var sale = await saleRepository.FindByIdAsync(domainEvent.SaleId.Value, cancellationToken);
        if (sale is null)
        {
            logger.LogWarning("Sale {SaleId} not found while notifying completion", domainEvent.SaleId);
            return;
        }

        var message = $"Sale #{sale.Id} for {sale.PatientName} has been completed and paid in full.";
        var command = new CreateSystemNotificationCommand(sale.UserId, NotificationCategory.SaleCompleted, message);
        var result  = await notificationCommandService.Handle(command, cancellationToken);

        if (result is Result<SystemNotification, CreateSystemNotificationError>.Failure failure)
            logger.LogError("Could not create sale completion notification for sale {SaleId}: {Error}",
                domainEvent.SaleId, failure.Error);
    }
}
