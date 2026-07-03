using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.Subscription.Interfaces.REST.Resources;

[SwaggerSchema(Description = "Stripe Checkout Session response")]
public record CheckoutSessionResource(
    [SwaggerParameter(Description = "Stripe Checkout Session URL to redirect the user to")] string CheckoutUrl);