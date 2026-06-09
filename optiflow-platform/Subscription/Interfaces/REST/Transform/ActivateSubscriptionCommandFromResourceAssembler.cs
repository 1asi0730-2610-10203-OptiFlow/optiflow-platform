using optiflow_platform.Subscription.Domain.Model.Commands;
using optiflow_platform.Subscription.Interfaces.REST.Resources;

namespace optiflow_platform.Subscription.Interfaces.REST.Transform;

public static class ActivateSubscriptionCommandFromResourceAssembler
{
    public static ActivateSubscriptionCommand ToCommandFromResource(
        int subscriptionId, ActivateSubscriptionResource resource) =>
        new(subscriptionId, resource.Tier, resource.StartDate, resource.EndDate);
}
