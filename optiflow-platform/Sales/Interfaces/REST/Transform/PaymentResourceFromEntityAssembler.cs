using optiflow_platform.Sales.Domain.Model.Aggregates;
using optiflow_platform.Sales.Interfaces.REST.Resources;

namespace optiflow_platform.Sales.Interfaces.REST.Transform;

public static class PaymentResourceFromEntityAssembler
{
    public static PaymentResource ToResourceFromEntity(Payment payment) =>
        new(payment.Id, payment.SaleId, payment.PaidAmount, payment.Method, payment.PaidAt);
}
