namespace optiflow_platform.Inventory.Domain.Model.Commands;

/// <summary>
///     Command to set the absolute stock level of a product, e.g. after a physical recount.
/// </summary>
public record UpdateStockLevelCommand(int ProductId, int NewStock);
