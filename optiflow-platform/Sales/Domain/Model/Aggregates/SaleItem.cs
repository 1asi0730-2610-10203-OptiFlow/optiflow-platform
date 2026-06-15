namespace optiflow_platform.Sales.Domain.Model.Aggregates;

public class SaleItem
{
    protected SaleItem()
    {
        Name = null!;
    }

    public SaleItem(int saleId, string name, int quantity, decimal unitPrice)
    {
        SaleId = saleId;
        Name = name;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Subtotal = quantity * unitPrice;
    }

    public int Id { get; private set; }
    public int SaleId { get; private set; }
    public string Name { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Subtotal { get; private set; }
}
