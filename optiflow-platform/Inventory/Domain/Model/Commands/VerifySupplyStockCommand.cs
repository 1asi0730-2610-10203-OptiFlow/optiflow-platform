namespace optiflow_platform.Inventory.Domain.Model.Commands;

/// <summary>
///     Command issued by the system to verify that a product has stock available to fulfill a supply request.
/// </summary>
public record VerifySupplyStockCommand(int ProductId);
