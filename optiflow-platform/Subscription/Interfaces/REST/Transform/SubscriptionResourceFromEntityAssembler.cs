using optiflow_platform.Subscription.Domain.Model.Aggregates;
using optiflow_platform.Subscription.Interfaces.REST.Resources;

namespace optiflow_platform.Subscription.Interfaces.REST.Transform;

public static class SubscriptionResourceFromEntityAssembler
{
    public static SubscriptionResource ToResourceFromEntity(Subscription subscription) =>
        new(subscription.Id, subscription.AdminId, subscription.PlanId, subscription.Tier,
            subscription.Amount, subscription.PaymentMethod, subscription.Status,
            subscription.StartDate, subscription.EndDate);
}
