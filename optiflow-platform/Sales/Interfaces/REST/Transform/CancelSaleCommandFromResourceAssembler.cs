using optiflow_platform.Sales.Domain.Model.Commands;
using optiflow_platform.Sales.Interfaces.REST.Resources;

namespace optiflow_platform.Sales.Interfaces.REST.Transform;

public static class CancelSaleCommandFromResourceAssembler
{
    public static CancelSaleCommand ToCommandFromResource(int saleId, CancelSaleResource resource) =>
        new(saleId, resource.LabOrderStatus);
}
