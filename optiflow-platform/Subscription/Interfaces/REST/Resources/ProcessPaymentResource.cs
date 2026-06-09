using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.Subscription.Interfaces.REST.Resources;

[SwaggerSchema(Description = "Payload to process a subscription payment")]
public record ProcessPaymentResource(
    [Required] [SwaggerParameter(Description = "Amount to charge")] decimal Amount,
    [Required] [SwaggerParameter(Description = "Payment method")] string PaymentMethod);
