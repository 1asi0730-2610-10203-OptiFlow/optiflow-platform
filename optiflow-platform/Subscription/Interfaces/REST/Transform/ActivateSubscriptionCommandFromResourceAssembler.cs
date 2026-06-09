using optiflow_platform.Subscription.Domain.Model.Commands;
using optiflow_platform.Subscription.Domain.Model.ValueObjects;
using optiflow_platform.Subscription.Interfaces.REST.Resources;

namespace optiflow_platform.Subscription.Interfaces.REST.Transform;

public static class ActivateSubscriptionCommandFromResourceAssembler
{
    public static ActivateSubscriptionCommand ToCommandFromResource(
        int subscriptionId, ActivateSubscriptionResource resource) =>
        new(new SubscriptionId(subscriptionId), new SubscriptionTier(resource.Tier), resource.StartDate, resource.EndDate);
}
