namespace optiflow_platform.Subscription.Application.Internal.OutboundServices.Stripe;

/// <summary>Outbound port for Stripe Checkout Session creation.</summary>
public interface IStripeCheckoutService
{
    /// <summary>
    /// Creates a Stripe Checkout Session for a pending subscription. The subscription id and account
    /// id are stored in the session metadata so the webhook can activate the right subscription.
    /// </summary>
    /// <param name="subscriptionId">The pending subscription's id.</param>
    /// <param name="accountId">The account the subscription belongs to.</param>
    /// <param name="planName">The display name of the plan.</param>
    /// <param name="amount">The payment amount in USD.</param>
    /// <param name="returnUrl">Absolute URL Stripe redirects the browser to after a successful payment.</param>
    /// <returns>The Stripe Checkout Session URL.</returns>
    string CreateCheckoutSession(int subscriptionId, Guid accountId, string planName, decimal amount, string returnUrl);
}