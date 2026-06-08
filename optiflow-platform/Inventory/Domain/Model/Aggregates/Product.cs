using optiflow_platform.Inventory.Domain.Model.Commands;

namespace optiflow_platform.Inventory.Domain.Model.Aggregates;

/// <summary>
///     Product aggregate root representing a catalog item and its stock level.
/// </summary>
/// <remarks>
///     A product is registered into the catalog with an initial stock and minimum
///     stock threshold, and its stock evolves through restocks, manual adjustments,
///     and absolute stock-level corrections.
/// </remarks>
public class Product
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected Product()
    {
        Category = null!;
        SupplierName = null!;
        Sku = null!;
        Name = null!;
        Brand = null!;
        Model = null!;
        LastRestockDate = null!;
    }

    /// <summary>
    ///     Registers a new product from a registration command.
    /// </summary>
    public Product(RegisterProductCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        CategoryId = command.CategoryId;
        Category = command.Category;
        SupplierId = command.SupplierId;
        SupplierName = command.SupplierName;
        Sku = command.Sku;
        Name = command.Name;
        Brand = command.Brand;
        Model = command.Model;
        Price = command.Price;
        Stock = command.Stock;
        MinimumStockThreshold = command.MinimumStockThreshold;
        LastRestockDate = command.LastRestockDate;
    }

    public int Id { get; private set; }
    public int CategoryId { get; private set; }
    public string Category { get; private set; }
    public int SupplierId { get; private set; }
    public string SupplierName { get; private set; }
    public string Sku { get; private set; }
    public string Name { get; private set; }
    public string Brand { get; private set; }
    public string Model { get; private set; }
    public decimal Price { get; private set; }
    public int Stock { get; private set; }
    public int MinimumStockThreshold { get; private set; }
    public string LastRestockDate { get; private set; }

    /// <summary>
    ///     Updates the catalog details of the product.
    /// </summary>
    public void UpdateDetails(UpdateProductCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        Name = command.Name;
        Sku = command.Sku;
        Category = command.Category;
        Price = command.Price;
        MinimumStockThreshold = command.MinimumStockThreshold;
    }

    /// <summary>
    ///     Sets the stock to an absolute level, e.g. after a physical recount.
    /// </summary>
    public void UpdateStockLevel(UpdateStockLevelCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        Stock = command.NewStock;
    }

    /// <summary>
    ///     Adds replenished stock and refreshes the last restock date.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when the quantity is zero or negative.</exception>
    public void Restock(RestockProductCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (command.Quantity <= 0)
            throw new ArgumentException("Restock quantity must be greater than zero.", nameof(command));
        Stock += command.Quantity;
        LastRestockDate = DateTimeOffset.UtcNow.ToString("yyyy-MM-dd");
    }

    /// <summary>
    ///     Confirms a manual stock correction backed by a justification.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when no justification is provided.</exception>
    public void AdjustStock(LogManualAdjustmentCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (string.IsNullOrWhiteSpace(command.Justification))
            throw new ArgumentException("A justification is required to confirm a stock adjustment.", nameof(command));
        Stock = command.NewStock;
    }

    /// <summary>
    ///     Determines whether the stock is at or below the minimum stock threshold.
    /// </summary>
    public bool IsLowStock() => Stock <= MinimumStockThreshold;

    /// <summary>
    ///     Determines whether the product currently has stock available.
    /// </summary>
    public bool HasStock() => Stock > 0;
}
