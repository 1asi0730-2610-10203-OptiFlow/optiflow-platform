namespace optiflow_platform.LabAndOrders.Domain.Model.Commands;

/// <summary>
///     Command to update the status of an existing work order.
/// </summary>
public record UpdateOrderStatusCommand(int WorkOrderId, string Status);
