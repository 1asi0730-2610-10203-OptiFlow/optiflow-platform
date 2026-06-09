using optiflow_platform.Subscription.Domain.Model.Aggregates;
using optiflow_platform.Subscription.Interfaces.REST.Resources;

namespace optiflow_platform.Subscription.Interfaces.REST.Transform;

public static class SubscriptionResourceFromEntityAssembler
{
    public static SubscriptionResource ToResourceFromEntity(Domain.Model.Aggregates.Subscription subscription) =>
        new(subscription.Id, subscription.AdminId, subscription.PlanId.Value, subscription.Tier.Value,
            subscription.Amount, subscription.PaymentMethod, subscription.Status.Value,
            subscription.StartDate, subscription.EndDate);
}
