using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.Sales.Interfaces.REST.Resources;

[SwaggerSchema(Description = "Payload to generate a sale quota")]
public record GenerateSaleQuotaResource(
    [Required] [SwaggerParameter(Description = "Advance payment amount (must be at least 30% of total)")] decimal Advance);
