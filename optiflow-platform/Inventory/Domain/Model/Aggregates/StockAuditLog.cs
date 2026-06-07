namespace optiflow_platform.Inventory.Domain.Model.Aggregates;

/// <summary>
///     Stock audit log aggregate root recording a single stock-changing operation for traceability.
/// </summary>
/// <remarks>
///     Created as a side effect of restocking or confirming a manual adjustment, capturing
///     the product state before and after the change. Backs the Inventory Audit History View
///     and Restock Audit Logged read models.
/// </remarks>
public class StockAuditLog
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected StockAuditLog()
    {
        ProductName = null!;
        Sku = null!;
        Operation = null!;
        Author = null!;
        Date = null!;
        Time = null!;
    }

    /// <summary>
    ///     Records a stock-changing operation performed on a product.
    /// </summary>
    public StockAuditLog(int productId, string productName, string sku, string operation,
        int previousStock, int quantity, int newStock, string author)
    {
        ProductId = productId;
        ProductName = productName;
        Sku = sku;
        Operation = operation;
        PreviousStock = previousStock;
        Quantity = quantity;
        NewStock = newStock;
        Author = author;

        var recordedAt = DateTimeOffset.UtcNow;
        Date = recordedAt.ToString("yyyy-MM-dd");
        Time = recordedAt.ToString("HH:mm:ss");
    }

    public int Id { get; private set; }
    public int ProductId { get; private set; }
    public string ProductName { get; private set; }
    public string Sku { get; private set; }
    public string Operation { get; private set; }
    public int PreviousStock { get; private set; }
    public int Quantity { get; private set; }
    public int NewStock { get; private set; }
    public string Author { get; private set; }
    public string Date { get; private set; }
    public string Time { get; private set; }
}
