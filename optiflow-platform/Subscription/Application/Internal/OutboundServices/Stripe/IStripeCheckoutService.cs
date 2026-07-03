namespace optiflow_platform.Subscription.Application.Internal.OutboundServices.Stripe;

/// <summary>Outbound port for Stripe Checkout Session creation.</summary>
public interface IStripeCheckoutService
{
    /// <summary>
    /// Creates a Stripe Checkout Session for a subscription payment.
    /// </summary>
    /// <param name="adminId">The admin user ID.</param>
    /// <param name="planId">The subscription plan ID.</param>
    /// <param name="planName">The display name of the plan.</param>
    /// <param name="amount">The payment amount in USD.</param>
    /// <returns>The Stripe Checkout Session URL.</returns>
    string CreateCheckoutSession(int adminId, int planId, string planName, decimal amount);
}