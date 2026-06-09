namespace optiflow_platform.Subscription.Domain.Model.Commands;

public record SelectSubscriptionPlanCommand(int AdminId, string PlanId, string Tier, decimal Amount, string PaymentMethod);

