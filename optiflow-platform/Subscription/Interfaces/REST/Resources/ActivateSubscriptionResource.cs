using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.Subscription.Interfaces.REST.Resources;

[SwaggerSchema(Description = "Payload to activate a subscription after payment is confirmed")]
public record ActivateSubscriptionResource(
    [Required] [SwaggerParameter(Description = "Tier to assign: BASIC, STANDARD, or PREMIUM")] string Tier,
    [Required] [SwaggerParameter(Description = "Subscription start date (ISO 8601)")] DateTimeOffset StartDate,
    [Required] [SwaggerParameter(Description = "Subscription end date (ISO 8601)")] DateTimeOffset EndDate);
