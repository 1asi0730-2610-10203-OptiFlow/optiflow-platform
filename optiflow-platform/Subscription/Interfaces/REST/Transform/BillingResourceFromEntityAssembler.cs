using optiflow_platform.Subscription.Domain.Model.Aggregates;
using optiflow_platform.Subscription.Interfaces.REST.Resources;

namespace optiflow_platform.Subscription.Interfaces.REST.Transform;

public static class BillingResourceFromEntityAssembler
{
    public static BillingResource ToResourceFromEntity(Billing billing) =>
        new(billing.Id, billing.SubscriptionId, billing.RenewalDate,
            billing.AutoRenew, billing.BillingStatus);
}
