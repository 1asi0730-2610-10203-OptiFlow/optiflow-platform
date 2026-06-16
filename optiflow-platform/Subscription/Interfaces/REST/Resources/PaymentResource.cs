using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.Subscription.Interfaces.REST.Resources;

[SwaggerSchema(Description = "Subscription payment response payload")]
public record SubscriptionPaymentResource(
    [SwaggerParameter(Description = "Payment identifier")] int Id,
    [SwaggerParameter(Description = "Related subscription identifier")] int SubscriptionId,
    [SwaggerParameter(Description = "Amount charged")] decimal Amount,
    [SwaggerParameter(Description = "Payment method")] string PaymentMethod,
    [SwaggerParameter(Description = "Payment status")] string Status,
    [SwaggerParameter(Description = "Timestamp when processed")] DateTimeOffset? ProcessedAt);
