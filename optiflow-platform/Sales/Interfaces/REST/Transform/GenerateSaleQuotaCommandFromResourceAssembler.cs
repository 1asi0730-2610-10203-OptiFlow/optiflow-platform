using optiflow_platform.Sales.Domain.Model.Commands;
using optiflow_platform.Sales.Interfaces.REST.Resources;

namespace optiflow_platform.Sales.Interfaces.REST.Transform;

public static class GenerateSaleQuotaCommandFromResourceAssembler
{
    public static GenerateSaleQuotaCommand ToCommandFromResource(int saleId, GenerateSaleQuotaResource resource) =>
        new(saleId, resource.Adelanto);
}
