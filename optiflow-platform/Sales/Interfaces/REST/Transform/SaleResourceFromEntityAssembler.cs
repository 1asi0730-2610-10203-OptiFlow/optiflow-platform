using optiflow_platform.Sales.Domain.Model.Aggregates;
using optiflow_platform.Sales.Interfaces.REST.Resources;

namespace optiflow_platform.Sales.Interfaces.REST.Transform;

public static class SaleResourceFromEntityAssembler
{
    public static SaleResource ToResourceFromEntity(Sale sale) =>
        new(sale.Id,
            sale.InvoiceNumber,
            sale.LabOrderNumber,
            sale.PatientId,
            sale.PatientName,
            sale.PatientRx,
            sale.UserId,
            sale.UserName,
            sale.Items.Select(i => new SaleItemResource(i.Id, i.Name, i.Quantity, i.UnitPrice, i.Subtotal)),
            sale.TotalAmount,
            sale.Adelanto,
            sale.PendingBalance,
            sale.DiscountCode,
            sale.DiscountAmount,
            sale.Status,
            sale.PaymentMethod,
            sale.CreatedAt,
            sale.DeliveredAt,
            sale.Notes);
}
