using optiflow_platform.Subscription.Domain.Model.ValueObjects;

namespace optiflow_platform.Subscription.Domain.Model.Commands;

/// <summary>
/// Issued when admin explicitely selects a plan
/// </summary>
/// <param name="AdminId"></param>
/// <param name="PlanId"></param>
/// <param name="Tier"></param>
/// <param name="Amount"></param>
/// <param name="PaymentMethod"></param>
public record SelectSubscriptionPlanCommand(int AdminId, PlanId PlanId, SubscriptionTier Tier, decimal Amount, string PaymentMethod);

