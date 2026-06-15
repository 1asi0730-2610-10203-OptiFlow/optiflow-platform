using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.Sales.Interfaces.REST.Resources;

[SwaggerSchema(Description = "Payload to pay the outstanding balance")]
public record PayOutstandingBalanceResource(
    [Required] [SwaggerParameter(Description = "Amount being paid")] decimal AmountPaid,
    [Required] [SwaggerParameter(Description = "Payment method (e.g. CASH, CARD, TRANSFER)")] string Method);
