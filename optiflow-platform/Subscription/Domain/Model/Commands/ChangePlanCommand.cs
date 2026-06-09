using optiflow_platform.Subscription.Domain.Model.ValueObjects;

namespace optiflow_platform.Subscription.Domain.Model.Commands;

/// <summary>
///     Issued when an admin requests a plan upgrade or downgrade.
///     Resets the subscription to PENDING_PAYMENT until the new payment is confirmed.
/// </summary>
/// <param name="SubscriptionId"></param>
/// <param name="NewPlanId"></param>
/// <param name="NewTier"></param>
/// <param name="Amount"></param>
/// <param name="PaymentMethod"></param>
public record ChangePlanCommand(
    SubscriptionId SubscriptionId,
    PlanId NewPlanId,
    SubscriptionTier NewTier,
    decimal Amount,
    string PaymentMethod);
