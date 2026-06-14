using optiflow_platform.Subscription.Domain.Model.Aggregates;
using optiflow_platform.Subscription.Interfaces.REST.Resources;

namespace optiflow_platform.Subscription.Interfaces.REST.Transform;

public static class PlanResourceFromEntityAssembler
{
    public static PlanResource ToResourceFromEntity(Plan plan) =>
        new(plan.Id, plan.Name, plan.Tier.Value, plan.Price, plan.Description);
}
