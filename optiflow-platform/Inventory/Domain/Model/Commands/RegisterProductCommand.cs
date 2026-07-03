using optiflow_platform.Inventory.Domain.Model.ValueObjects;

namespace optiflow_platform.Inventory.Domain.Model.Commands;

/// <summary>
///     Command to register a new product in the catalog.
/// </summary>
public record RegisterProductCommand(
    EProductCategory Category,
    int SupplierId,
    string SupplierName,
    string Sku,
    string Name,
    string Brand,
    string Model,
    decimal Price,
    int Stock,
    int MinimumStockThreshold,
    string LastRestockDate);
