using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.Subscription.Interfaces.REST.Resources;

[SwaggerSchema(Description = "Subscription response payload")]
public record SubscriptionResource(
    [SwaggerParameter(Description = "Subscription identifier")] int Id,
    [SwaggerParameter(Description = "Admin owner identifier")] int AdminId,
    [SwaggerParameter(Description = "Plan identifier")] int PlanId,
    [SwaggerParameter(Description = "Subscription tier")] string Tier,
    [SwaggerParameter(Description = "Monthly amount")] decimal Amount,
    [SwaggerParameter(Description = "Payment method")] string PaymentMethod,
    [SwaggerParameter(Description = "Current status")] string Status,
    [SwaggerParameter(Description = "Start date")] DateTimeOffset? StartDate,
    [SwaggerParameter(Description = "End date")] DateTimeOffset? EndDate);
