using optiflow_platform.Subscription.Domain.Model.Commands;
using optiflow_platform.Subscription.Domain.Model.ValueObjects;
using optiflow_platform.Subscription.Interfaces.REST.Resources;

namespace optiflow_platform.Subscription.Interfaces.REST.Transform;

public static class SelectSubscriptionPlanCommandFromResourceAssembler
{
    public static SelectSubscriptionPlanCommand ToCommandFromResource(SelectSubscriptionPlanResource resource) =>
        new(resource.AdminId, new PlanId(resource.PlanId), new SubscriptionTier(resource.Tier), resource.Amount, resource.PaymentMethod);
}
