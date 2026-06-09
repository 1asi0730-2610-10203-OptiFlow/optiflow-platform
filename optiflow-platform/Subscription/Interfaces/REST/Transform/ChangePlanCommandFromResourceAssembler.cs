using optiflow_platform.Subscription.Domain.Model.Commands;
using optiflow_platform.Subscription.Interfaces.REST.Resources;

namespace optiflow_platform.Subscription.Interfaces.REST.Transform;

public static class ChangePlanCommandFromResourceAssembler
{
    public static ChangePlanCommand ToCommandFromResource(
        int subscriptionId, ChangePlanResource resource) =>
        new(subscriptionId, resource.NewPlanId, resource.NewTier, resource.Amount, resource.PaymentMethod);
}
