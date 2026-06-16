using optiflow_platform.Subscription.Domain.Model.ValueObjects;

namespace optiflow_platform.Subscription.Domain.Model.Commands;

/// <summary>
///     Issued when an admin creates a new subscription plan.
/// </summary>
/// <param name="Name"></param>
/// <param name="Tier"></param>
/// <param name="Price"></param>
/// <param name="Description"></param>
public record CreatePlanCommand(
    string Name,
    SubscriptionTier Tier,
    decimal Price,
    string Description);
