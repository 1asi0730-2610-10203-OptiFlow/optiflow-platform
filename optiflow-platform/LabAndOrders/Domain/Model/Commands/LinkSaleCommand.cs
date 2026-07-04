namespace optiflow_platform.LabAndOrders.Domain.Model.Commands;

/// <summary>
///     Command to link a work order to the sale created for it.
/// </summary>
public record LinkSaleCommand(int WorkOrderId, int SaleId);
