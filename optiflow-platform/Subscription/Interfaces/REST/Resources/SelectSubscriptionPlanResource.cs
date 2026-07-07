using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.Subscription.Interfaces.REST.Resources;

[SwaggerSchema(Description = "Payload to select a subscription plan and create a new subscription")]
public record SelectSubscriptionPlanResource(
    [Required] [SwaggerParameter(Description = "ID of the admin creating the subscription")] int AdminId,
    [Required] [SwaggerParameter(Description = "Plan identifier")] int PlanId,
    [Required] [SwaggerParameter(Description = "Tier: BASIC, STANDARD, or PREMIUM")] string Tier,
    [Required] [Range(0.01, 1000000, ErrorMessage = "Amount must be between 0.01 and 1000000")] [SwaggerParameter(Description = "Monthly subscription amount")] decimal Amount,
    [Required] [SwaggerParameter(Description = "Payment method (e.g. CREDIT_CARD)")] string PaymentMethod);
