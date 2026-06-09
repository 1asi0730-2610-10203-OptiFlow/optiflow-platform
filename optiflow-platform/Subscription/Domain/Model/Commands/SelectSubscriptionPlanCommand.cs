namespace optiflow_platform.Subscription.Domain.Model.Commands;

/// <summary>
/// Issued when admin explicitely selects a plan
/// </summary>
/// <param name="AdminId"></param>
/// <param name="PlanId"></param>
/// <param name="Tier"></param>
/// <param name="Amount"></param>
/// <param name="PaymentMethod"></param>
public record SelectSubscriptionPlanCommand(int AdminId, string PlanId, string Tier, decimal Amount, string PaymentMethod);

