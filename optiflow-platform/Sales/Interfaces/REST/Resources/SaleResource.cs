using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.Sales.Interfaces.REST.Resources;

[SwaggerSchema(Description = "A sale resource")]
public record SaleResource(
    [SwaggerParameter(Description = "The server-generated ID of the sale")] int Id,
    [SwaggerParameter(Description = "Full name of the client")] string ClientName,
    [SwaggerParameter(Description = "Total amount of the sale after any discount")] decimal TotalAmount,
    [SwaggerParameter(Description = "Advance quota amount (minimum 30% of full sale)")] decimal QuotaAmount,
    [SwaggerParameter(Description = "Promotional discount percentage applied")] decimal DiscountPercentage,
    [SwaggerParameter(Description = "Current status of the sale")] string Status,
    [SwaggerParameter(Description = "Date the sale was created")] string SaleDate);
