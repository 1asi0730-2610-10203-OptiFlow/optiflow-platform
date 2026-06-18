using optiflow_platform.Sales.Domain.Model.Commands;
using optiflow_platform.Sales.Domain.Model.ValueObjects;
using optiflow_platform.Sales.Interfaces.REST.Resources;

namespace optiflow_platform.Sales.Interfaces.REST.Transform;

public static class PayOutstandingBalanceCommandFromResourceAssembler
{
    public static PayOutstandingBalanceCommand ToCommandFromResource(SaleId saleId,
        PayOutstandingBalanceResource resource) =>
        new(saleId, resource.AmountPaid, resource.Method);
}
