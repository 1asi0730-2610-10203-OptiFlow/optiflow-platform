using optiflow_platform.Subscription.Domain.Model.Aggregates;
using optiflow_platform.Subscription.Interfaces.REST.Resources;

namespace optiflow_platform.Subscription.Interfaces.REST.Transform;

public static class PaymentResourceFromEntityAssembler
{
    public static PaymentResource ToResourceFromEntity(Payment payment) =>
        new(payment.Id, payment.SubscriptionId, payment.Amount,
            payment.PaymentMethod, payment.Status, payment.ProcessedAt);
}
