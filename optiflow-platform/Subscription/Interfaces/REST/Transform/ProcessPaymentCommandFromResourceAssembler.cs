using optiflow_platform.Subscription.Domain.Model.Commands;
using optiflow_platform.Subscription.Interfaces.REST.Resources;

namespace optiflow_platform.Subscription.Interfaces.REST.Transform;

public static class ProcessPaymentCommandFromResourceAssembler
{
    public static ProcessSubscriptionPaymentCommand ToCommandFromResource(
        int subscriptionId, ProcessPaymentResource resource) =>
        new(subscriptionId, resource.Amount, resource.PaymentMethod);
}
