namespace optiflow_platform.Inventory.Domain.Model.Commands;

/// <summary>
///     Command to add stock to a product after a supplier replenishment.
/// </summary>
public record RestockProductCommand(int ProductId, int Quantity, string Author);
