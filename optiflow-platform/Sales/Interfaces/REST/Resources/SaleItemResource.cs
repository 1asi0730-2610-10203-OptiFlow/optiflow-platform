using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.Sales.Interfaces.REST.Resources;

[SwaggerSchema(Description = "A line item within a sale")]
public record SaleItemResource(
    [SwaggerParameter(Description = "Item ID")] int Id,
    [SwaggerParameter(Description = "Item name")] string Name,
    [SwaggerParameter(Description = "Quantity")] int Quantity,
    [SwaggerParameter(Description = "Unit price")] decimal UnitPrice,
    [SwaggerParameter(Description = "Line subtotal")] decimal Subtotal);
