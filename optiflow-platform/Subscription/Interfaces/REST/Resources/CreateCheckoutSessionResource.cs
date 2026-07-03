using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.Subscription.Interfaces.REST.Resources;

[SwaggerSchema(Description = "Payload to create a Stripe Checkout Session")]
public record CreateCheckoutSessionResource(
    [Required][SwaggerParameter(Description = "ID of the admin user")] int AdminId,
    [Required][SwaggerParameter(Description = "ID of the selected plan")] int PlanId,
    [Required][SwaggerParameter(Description = "Display name of the plan")] string PlanName,
    [Required][SwaggerParameter(Description = "Amount to charge in USD")] decimal Amount);