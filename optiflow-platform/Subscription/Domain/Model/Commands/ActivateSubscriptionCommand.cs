namespace optiflow_platform.Subscription.Domain.Model.Commands;


public record ActivateSubscriptionCommand(int SubscriptionId, string Tier, DateTimeOffset StartDate, DateTimeOffset EndDate);