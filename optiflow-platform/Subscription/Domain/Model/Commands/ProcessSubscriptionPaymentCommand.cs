using optiflow_platform.Subscription.Domain.Model.ValueObjects;

namespace optiflow_platform.Subscription.Domain.Model.Commands;

/// <summary>
///  Issued to charge the payment for a given subscription.
/// </summary>
/// <param name="SubscriptionId"></param>
/// <param name="Amount"></param>
/// <param name="PaymentMethod"></param>
public record ProcessSubscriptionPaymentCommand(
    SubscriptionId SubscriptionId,
    decimal Amount,
    string PaymentMethod);