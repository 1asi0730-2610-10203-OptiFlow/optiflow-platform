using optiflow_platform.Sales.Domain.Model.Aggregates;
using optiflow_platform.Sales.Interfaces.REST.Resources;

namespace optiflow_platform.Sales.Interfaces.REST.Transform;

public static class SaleResourceFromEntityAssembler
{
    public static SaleResource ToResourceFromEntity(Sale sale) =>
        new(sale.Id, sale.ClientName, sale.TotalAmount, sale.QuotaAmount,
            sale.DiscountPercentage, sale.Status, sale.SaleDate);
}
