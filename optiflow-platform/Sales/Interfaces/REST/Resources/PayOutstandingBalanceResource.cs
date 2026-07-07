using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.Sales.Interfaces.REST.Resources;

[SwaggerSchema(Description = "Payload to pay the outstanding balance")]
public record PayOutstandingBalanceResource(
    [Required] [Range(0.01, 1000000, ErrorMessage = "AmountPaid must be between 0.01 and 1000000")] [SwaggerParameter(Description = "Amount being paid")] decimal AmountPaid,
    [Required] [StringLength(50, ErrorMessage = "Method must be at most 50 characters")] [SwaggerParameter(Description = "Payment method (e.g. CASH, CARD, TRANSFER)")] string Method);
