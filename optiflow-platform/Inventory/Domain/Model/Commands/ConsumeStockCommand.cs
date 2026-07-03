namespace optiflow_platform.Inventory.Domain.Model.Commands;

/// <summary>
///     Command to deduct stock consumed to fulfill an external order, e.g. lab order material.
/// </summary>
public record ConsumeStockCommand(int ProductId, int Quantity, string Author);
