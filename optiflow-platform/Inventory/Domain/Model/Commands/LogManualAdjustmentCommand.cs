namespace optiflow_platform.Inventory.Domain.Model.Commands;

/// <summary>
///     Command to confirm a manual stock correction backed by a justification.
/// </summary>
public record LogManualAdjustmentCommand(int ProductId, int NewStock, string Justification, string Author);
