using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.Subscription.Interfaces.REST.Resources;

[SwaggerSchema(Description = "Payload to request a subscription plan change")]
public record ChangePlanResource(
    [Required] [SwaggerParameter(Description = "New plan identifier")] int NewPlanId,
    [Required] [SwaggerParameter(Description = "New tier")] string NewTier,
    [Required] [Range(0.01, 1000000, ErrorMessage = "Amount must be between 0.01 and 1000000")] [SwaggerParameter(Description = "New monthly amount")] decimal Amount,
    [Required] [SwaggerParameter(Description = "Payment method for the new plan")] string PaymentMethod);
