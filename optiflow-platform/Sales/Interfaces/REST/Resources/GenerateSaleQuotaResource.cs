using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.Sales.Interfaces.REST.Resources;

[SwaggerSchema(Description = "Payload to generate a sale quota")]
public record GenerateSaleQuotaResource(
    [Required] [Range(0.01, 1000000, ErrorMessage = "Advance must be between 0.01 and 1000000")] [SwaggerParameter(Description = "Advance payment amount (must be at least 30% of total)")] decimal Advance);
