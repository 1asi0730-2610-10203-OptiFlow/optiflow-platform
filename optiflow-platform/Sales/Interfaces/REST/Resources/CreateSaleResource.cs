using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.Sales.Interfaces.REST.Resources;

[SwaggerSchema(Description = "Payload to create a new sale")]
public record CreateSaleResource(
    [Required] [SwaggerParameter(Description = "Full name of the client")] string ClientName,
    [Required] [SwaggerParameter(Description = "Total amount of the sale")] decimal TotalAmount,
    [Required] [SwaggerParameter(Description = "Date of the sale (ISO 8601)")] string SaleDate);
