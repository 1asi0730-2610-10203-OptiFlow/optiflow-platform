using optiflow_platform.Sales.Domain.Model.Commands;
using optiflow_platform.Sales.Interfaces.REST.Resources;

namespace optiflow_platform.Sales.Interfaces.REST.Transform;

public static class ApplyPromotionalDiscountCommandFromResourceAssembler
{
    public static ApplyPromotionalDiscountCommand ToCommandFromResource(int saleId,
        ApplyPromotionalDiscountResource resource) =>
        new(saleId, resource.DiscountCode, resource.DiscountAmount);
}
