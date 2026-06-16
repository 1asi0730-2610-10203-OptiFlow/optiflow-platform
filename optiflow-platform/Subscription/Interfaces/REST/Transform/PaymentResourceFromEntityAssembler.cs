using optiflow_platform.Subscription.Domain.Model.Aggregates;
using optiflow_platform.Subscription.Interfaces.REST.Resources;

namespace optiflow_platform.Subscription.Interfaces.REST.Transform;

public static class SubscriptionPaymentResourceFromEntityAssembler
{
    public static SubscriptionPaymentResource ToResourceFromEntity(Payment payment) =>
        new(payment.Id, payment.SubscriptionId.Value, payment.Amount,
            payment.PaymentMethod, payment.Status.Value, payment.ProcessedAt);
}
