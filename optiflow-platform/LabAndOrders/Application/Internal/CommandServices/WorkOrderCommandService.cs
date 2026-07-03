using Cortex.Mediator;
using optiflow_platform.LabAndOrders.Application.Errors;
using optiflow_platform.LabAndOrders.Application.Services;
using optiflow_platform.LabAndOrders.Domain.Model.Aggregates;
using optiflow_platform.LabAndOrders.Domain.Model.Commands;
using optiflow_platform.LabAndOrders.Domain.Model.Events;
using optiflow_platform.LabAndOrders.Domain.Repositories;
using optiflow_platform.Shared.Application.Patterns;
using optiflow_platform.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace optiflow_platform.LabAndOrders.Application.Internal.CommandServices;

/// <summary>
///     Application service for handling work order commands.
/// </summary>
/// <remarks>
///     Handles creation and status-update commands for work orders.
///     Coordinates with the repository and unit of work to persist changes.
/// </remarks>
/// <param name="workOrderRepository">Repository for work order persistence.</param>
/// <param name="unitOfWork">Unit of work for transaction scope.</param>
/// <param name="domainEventPublisher">Publisher for work order domain events.</param>
/// <param name="logger">Logger for diagnostic and error reporting.</param>
public class WorkOrderCommandService(
    IWorkOrderRepository workOrderRepository,
    IUnitOfWork unitOfWork,
    IMediator domainEventPublisher,
    ILogger<WorkOrderCommandService> logger)
    : IWorkOrderCommandService
{
    /// <inheritdoc />
    public async Task<Result<WorkOrder, CreateWorkOrderError>> Handle(CreateWorkOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var workOrder = new WorkOrder(command);
            await workOrderRepository.AddAsync(workOrder, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            await domainEventPublisher.PublishAsync(
                new WorkOrderCreatedEvent(workOrder.Id, workOrder.LensProductId, workOrder.FrameProductId, workOrder.PatientName),
                cancellationToken);
            return new Result<WorkOrder, CreateWorkOrderError>.Success(workOrder);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Database error while creating work order for patient {PatientName}", command.PatientName);
            return new Result<WorkOrder, CreateWorkOrderError>.Failure(CreateWorkOrderError.UnexpectedError);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while creating work order for patient {PatientName}", command.PatientName);
            return new Result<WorkOrder, CreateWorkOrderError>.Failure(CreateWorkOrderError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<WorkOrder, UpdateOrderStatusError>> Handle(UpdateOrderStatusCommand command,
        CancellationToken cancellationToken = default)
    {
        var workOrder = await workOrderRepository.FindByIdAsync(command.WorkOrderId, cancellationToken);
        if (workOrder is null)
        {
            logger.LogWarning("Work order {WorkOrderId} not found for status update", command.WorkOrderId);
            return new Result<WorkOrder, UpdateOrderStatusError>.Failure(UpdateOrderStatusError.WorkOrderNotFound);
        }

        try
        {
            workOrder.UpdateStatus(command);
            workOrderRepository.Update(workOrder);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<WorkOrder, UpdateOrderStatusError>.Success(workOrder);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid status {Status} for work order {WorkOrderId}", command.Status, command.WorkOrderId);
            return new Result<WorkOrder, UpdateOrderStatusError>.Failure(UpdateOrderStatusError.InvalidStatus);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Database error updating status for work order {WorkOrderId}", command.WorkOrderId);
            return new Result<WorkOrder, UpdateOrderStatusError>.Failure(UpdateOrderStatusError.UnexpectedError);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error updating status for work order {WorkOrderId}", command.WorkOrderId);
            return new Result<WorkOrder, UpdateOrderStatusError>.Failure(UpdateOrderStatusError.UnexpectedError);
        }
    }
}
