namespace optiflow_platform.Sales.Domain.Model.Entities;

/// <summary>
///     A single product and quantity sold as part of a sale.
/// </summary>
/// <remarks>
///     Referenced by <see cref="SaleId"/>, not owned as an EF navigation on <c>Sale</c> —
///     same convention as <c>Payment.SaleId</c>. Read back by the Sales completion flow
///     to know what stock to deplete in the Inventory bounded context.
/// </remarks>
public class SaleItem
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected SaleItem()
    {
    }

    /// <summary>
    ///     Records a product and quantity sold as part of a sale.
    /// </summary>
    public SaleItem(int saleId, int productId, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
        SaleId = saleId;
        ProductId = productId;
        Quantity = quantity;
    }

    public int Id { get; private set; }
    public int SaleId { get; private set; }
    public int ProductId { get; private set; }
    public int Quantity { get; private set; }
}
