using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.Subscription.Interfaces.REST.Resources;

[SwaggerSchema(Description = "Billing response payload")]
public record BillingResource(
    [SwaggerParameter(Description = "Billing identifier")] int Id,
    [SwaggerParameter(Description = "Related subscription identifier")] int SubscriptionId,
    [SwaggerParameter(Description = "Next renewal date")] DateTimeOffset RenewalDate,
    [SwaggerParameter(Description = "Whether auto-renew is enabled")] bool AutoRenew,
    [SwaggerParameter(Description = "Billing status")] string BillingStatus);
