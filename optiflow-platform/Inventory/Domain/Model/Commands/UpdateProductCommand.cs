namespace optiflow_platform.Inventory.Domain.Model.Commands;

/// <summary>
///     Command to update the catalog details of an existing product.
/// </summary>
public record UpdateProductCommand(
    int ProductId,
    string Name,
    string Sku,
    string Category,
    decimal Price,
    int MinimumStockThreshold);
