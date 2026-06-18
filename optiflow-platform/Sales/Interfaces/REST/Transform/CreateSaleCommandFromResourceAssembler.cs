using optiflow_platform.Sales.Domain.Model.Commands;
using optiflow_platform.Sales.Domain.Model.ValueObjects;
using optiflow_platform.Sales.Interfaces.REST.Resources;

namespace optiflow_platform.Sales.Interfaces.REST.Transform;

public static class CreateSaleCommandFromResourceAssembler
{
    public static CreateSaleCommand ToCommandFromResource(CreateSaleResource resource) =>
        new(new InvoiceNumber(resource.InvoiceNumber),
            resource.LabOrderNumber,
            resource.PatientId,
            resource.PatientName,
            resource.UserId,
            resource.UserName,
            resource.TotalAmount,
            resource.Advance,
            resource.DiscountCode,
            resource.DiscountAmount,
            resource.PaymentMethod,
            resource.CreatedAt,
            resource.DeliveredAt,
            resource.Notes);
}
