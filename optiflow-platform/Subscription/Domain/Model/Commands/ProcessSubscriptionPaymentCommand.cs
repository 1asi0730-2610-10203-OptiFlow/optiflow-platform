namespace optiflow_platform.Subscription.Domain.Model.Commands;


public record ProcessSubscriptionPaymentCommand(
    int SubscriptionId,
    decimal Amount,
    string PaymentMethod);