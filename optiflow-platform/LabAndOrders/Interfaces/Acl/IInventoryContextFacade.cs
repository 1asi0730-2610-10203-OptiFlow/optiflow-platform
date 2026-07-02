namespace optiflow_platform.LabAndOrders.Interfaces.Acl;

/// <summary>
///     Anti-Corruption Layer facade exposing only what the LabAndOrders context needs from Inventory.
/// </summary>
public interface IInventoryContextFacade
{
    /// <summary>
    ///     Consumes stock from the given product to fulfill lab order material, e.g. lens or frame material.
    ///     Returns true if the stock was successfully consumed, false otherwise (product not found or insufficient stock).
    /// </summary>
    Task<bool> ConsumeStockAsync(int productId, int quantity, string author,
        CancellationToken cancellationToken = default);
}
