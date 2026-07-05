using optiflow_platform.Subscription.Application.Internal.OutboundServices.Stripe;
using Stripe;
using Stripe.Checkout;

namespace optiflow_platform.Subscription.Infrastructure.Stripe.Services;

/// <summary>Stripe implementation of IStripeCheckoutService.</summary>
public class StripeCheckoutService(IConfiguration configuration, ILogger<StripeCheckoutService> logger)
    : IStripeCheckoutService
{
    private readonly string _frontendUrl = configuration["AppSettings:FrontendUrl"]?.TrimEnd('/') ?? "http://localhost:5173";
    private readonly string? _successUrl = configuration["Stripe:SuccessUrl"];
    private readonly string? _cancelUrl  = configuration["Stripe:CancelUrl"];

    /// <inheritdoc />
    public string CreateCheckoutSession(int subscriptionId, Guid accountId, string planName, decimal amount, string returnUrl)
    {
        // Send the browser back through the backend so activation + redirect happen server-side.
        var successUrl = string.IsNullOrWhiteSpace(_successUrl) ? returnUrl : _successUrl;
        var cancelUrl  = string.IsNullOrWhiteSpace(_cancelUrl)  ? _frontendUrl + "/select-plan?status=cancelled" : _cancelUrl;

        var options = new SessionCreateOptions
        {
            Mode       = "payment",
            SuccessUrl = successUrl + (successUrl.Contains('?') ? "&" : "?") + "session_id={CHECKOUT_SESSION_ID}",
            CancelUrl  = cancelUrl,
            Metadata   = new Dictionary<string, string>
            {
                { "subscriptionId", subscriptionId.ToString() },
                { "accountId",      accountId.ToString()      }
            },
            LineItems =
            [
                new SessionLineItemOptions
                {
                    Quantity  = 1,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency   = "usd",
                        UnitAmount = (long)(amount * 100),
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name        = "OptiFlow — " + planName,
                            Description = "OptiFlow optical ERP subscription plan"
                        }
                    }
                }
            ]
        };

        try
        {
            var service = new SessionService();
            var session = service.Create(options);
            logger.LogInformation("Stripe Checkout Session created for subscription {SubscriptionId}: {SessionId}",
                subscriptionId, session.Id);
            return session.Url;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create Stripe Checkout Session for subscription {SubscriptionId}", subscriptionId);
            throw new InvalidOperationException("stripe.checkout.session.creation.failed");
        }
    }
}
