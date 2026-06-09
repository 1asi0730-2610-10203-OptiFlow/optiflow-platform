using optiflow_platform.Subscription.Domain.Model.Commands;
using optiflow_platform.Subscription.Domain.Model.ValueObjects;

namespace optiflow_platform.Subscription.Domain.Model.Aggregates;

/// <summary>
///     Aggregate root representing a subscription plan offering.
///     Subscriptions reference a Plan by its PlanId.
/// </summary>
public class Plan
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core materialisation.
    /// </summary>
    protected Plan()
    {
        Name        = null!;
        Tier        = null!;
        Description = null!;
    }

    /// <summary>
    ///     Creates a new plan from a creation command.
    /// </summary>
    public Plan(CreatePlanCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        Name        = command.Name;
        Tier        = command.Tier;
        Price       = command.Price;
        Description = command.Description;
    }

    public int              Id          { get; private set; }
    public string           Name        { get; private set; }
    public SubscriptionTier Tier        { get; private set; }
    public decimal          Price       { get; private set; }
    public string           Description { get; private set; }

    /// <summary>Gets the typed aggregate identity.</summary>
    public PlanId PlanId => new(Id);
}
