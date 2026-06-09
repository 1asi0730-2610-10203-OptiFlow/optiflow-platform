/// <summary>
///     Issued when an admin requests a plan upgrade or downgrade.
///     Resets the subscription to PENDING_PAYMENT until the new payment is confirmed.
/// </summary>
 
public record ChangePlanCommand(
    int SubscriptionId,
    string NewPlanId,
    string NewTier,
    decimal Amount,
    string PaymentMethod
    );
    
    