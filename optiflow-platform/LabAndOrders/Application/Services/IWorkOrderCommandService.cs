using optiflow_platform.LabAndOrders.Application.Errors;
using optiflow_platform.LabAndOrders.Domain.Model.Aggregates;
using optiflow_platform.LabAndOrders.Domain.Model.Commands;
using optiflow_platform.Shared.Application.Patterns;

namespace optiflow_platform.LabAndOrders.Application.Services;

/// <summary>
///     Contract for work order command operations.
/// </summary>
public interface IWorkOrderCommandService
{
    /// <summary>
    ///     Handles the creation of a new work order.
    /// </summary>
    Task<Result<WorkOrder, CreateWorkOrderError>> Handle(CreateWorkOrderCommand command,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Handles updating the status of an existing work order.
    /// </summary>
    Task<Result<WorkOrder, UpdateOrderStatusError>> Handle(UpdateOrderStatusCommand command,
        CancellationToken cancellationToken = default);
}
