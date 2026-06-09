using optiflow_platform.Subscription.Domain.Model.Commands;
using optiflow_platform.Subscription.Interfaces.REST.Resources;

namespace optiflow_platform.Subscription.Interfaces.REST.Transform;

public static class SelectSubscriptionPlanCommandFromResourceAssembler
{
    public static SelectSubscriptionPlanCommand ToCommandFromResource(SelectSubscriptionPlanResource resource) =>
        new(resource.AdminId, resource.PlanId, resource.Tier, resource.Amount, resource.PaymentMethod);
}
