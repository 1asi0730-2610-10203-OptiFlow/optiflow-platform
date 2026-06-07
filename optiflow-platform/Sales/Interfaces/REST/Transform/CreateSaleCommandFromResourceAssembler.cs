using optiflow_platform.Sales.Domain.Model.Commands;
using optiflow_platform.Sales.Interfaces.REST.Resources;

namespace optiflow_platform.Sales.Interfaces.REST.Transform;

public static class CreateSaleCommandFromResourceAssembler
{
    public static CreateSaleCommand ToCommandFromResource(CreateSaleResource resource) =>
        new(resource.ClientName, resource.TotalAmount, resource.SaleDate);
}
