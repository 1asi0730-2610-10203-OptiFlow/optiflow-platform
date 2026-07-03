namespace optiflow_platform.Inventory.Domain.Model.Commands;

/// <summary>
///     Command to remove stock from a product sold in a completed sale.
/// </summary>
public record ReduceStockCommand(int ProductId, int Quantity, string Reason);
