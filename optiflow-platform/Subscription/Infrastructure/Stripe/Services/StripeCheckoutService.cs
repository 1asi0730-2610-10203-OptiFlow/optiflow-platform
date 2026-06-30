using optiflow_platform.Subscription.Application.Internal.OutboundServices.Stripe;
using Stripe;
using Stripe.Checkout;

namespace optiflow_platform.Subscription.Infrastructure.Stripe.Services;

/// <summary>Stripe implementation of IStripeCheckoutService.</summary>
public class StripeCheckoutService(IConfiguration configuration, ILogger<StripeCheckoutService> logger)
    : IStripeCheckoutService
{
    private readonly string _successUrl = configuration["Stripe:SuccessUrl"]!;
    private readonly string _cancelUrl  = configuration["Stripe:CancelUrl"]!;

    /// <inheritdoc />
    public string CreateCheckoutSession(int adminId, int planId, string planName, decimal amount)
    {
        var options = new SessionCreateOptions
        {
            Mode       = "payment",
            SuccessUrl = _successUrl + "?session_id={CHECKOUT_SESSION_ID}",
            CancelUrl  = _cancelUrl,
            Metadata   = new Dictionary<string, string>
            {
                { "adminId", adminId.ToString() },
                { "planId",  planId.ToString()  }
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
            logger.LogInformation("Stripe Checkout Session created for admin {AdminId} plan {PlanId}: {SessionId}",
                adminId, planId, session.Id);
            return session.Url;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create Stripe Checkout Session for admin {AdminId}", adminId);
            throw new InvalidOperationException("stripe.checkout.session.creation.failed");
        }
    }
}