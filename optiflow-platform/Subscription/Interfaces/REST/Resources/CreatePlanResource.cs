namespace optiflow_platform.Subscription.Interfaces.REST.Resources;

public record CreatePlanResource(string Name, string Tier, decimal Price, string Description);
