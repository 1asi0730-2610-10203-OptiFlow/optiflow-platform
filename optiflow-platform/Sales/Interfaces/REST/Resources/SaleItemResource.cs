using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.Sales.Interfaces.REST.Resources;

[SwaggerSchema(Description = "A product line item sold as part of a sale")]
public record SaleItemResource(
    [SwaggerParameter(Description = "Inventory product ID")] int ProductId,
    [SwaggerParameter(Description = "Quantity sold")] int Quantity);
