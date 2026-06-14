namespace optiflow_platform.Subscription.Interfaces.REST.Resources;

public record PlanResource(int Id, string Name, string Tier, decimal Price, string Description);
