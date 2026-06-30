using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using optiflow_platform.Subscription.Application.Internal.OutboundServices.Stripe;
using optiflow_platform.Subscription.Interfaces.REST.Resources;
using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.Subscription.Interfaces.REST;

[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Checkout")]
public class CheckoutController(
    IStripeCheckoutService stripeCheckoutService,
    ILogger<CheckoutController> logger) : ControllerBase
{
    /// <summary>Creates a Stripe Checkout Session and returns the redirect URL.</summary>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Create Stripe Checkout Session",
        Description = "Creates a Stripe Checkout Session for a subscription payment and returns the URL.",
        OperationId = "CreateCheckoutSession")]
    [SwaggerResponse(200, "Checkout session created", typeof(CheckoutSessionResource))]
    [SwaggerResponse(500, "Failed to create checkout session")]
    public ActionResult<CheckoutSessionResource> CreateCheckoutSession(
        [FromBody] CreateCheckoutSessionResource resource)
    {
        try
        {
            var url = stripeCheckoutService.CreateCheckoutSession(
                resource.AdminId,
                resource.PlanId,
                resource.PlanName,
                resource.Amount);
            return Ok(new CheckoutSessionResource(url));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error creating checkout session for admin {AdminId}", resource.AdminId);
            return Problem(
                title: "Checkout error",
                detail: "Failed to create Stripe Checkout Session.",
                statusCode: 500);
        }
    }
}