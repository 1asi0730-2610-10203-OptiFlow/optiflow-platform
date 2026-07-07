using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.Subscription.Interfaces.REST.Resources;

[SwaggerSchema(Description = "Payload to create a Stripe Checkout Session")]
public record CreateCheckoutSessionResource(
    [Required][SwaggerParameter(Description = "ID of the admin user")] int AdminId,
    [Required][SwaggerParameter(Description = "ID of the selected plan")] int PlanId,
    [Required][StringLength(100, ErrorMessage = "PlanName must be at most 100 characters")][SwaggerParameter(Description = "Display name of the plan")] string PlanName,
    [Required][Range(0.01, 1000000, ErrorMessage = "Amount must be between 0.01 and 1000000")][SwaggerParameter(Description = "Amount to charge in USD")] decimal Amount);