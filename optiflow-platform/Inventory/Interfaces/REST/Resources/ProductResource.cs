using optiflow_platform.Inventory.Domain.Model.ValueObjects;
using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.Inventory.Interfaces.REST.Resources;

/// <summary>
///     Represents the data provided by the server about a product.
/// </summary>
[SwaggerSchema(Description = "A product resource")]
public record ProductResource(
    [SwaggerParameter(Description = "The server-generated ID of the product")] int Id,
    [SwaggerParameter(Description = "The product category")] EProductCategory Category,
    [SwaggerParameter(Description = "Reference to the supplier")] int SupplierId,
    [SwaggerParameter(Description = "Name of the supplier")] string SupplierName,
    [SwaggerParameter(Description = "Stock keeping unit code")] string Sku,
    [SwaggerParameter(Description = "Name of the product")] string Name,
    [SwaggerParameter(Description = "Brand of the product")] string Brand,
    [SwaggerParameter(Description = "Model of the product")] string Model,
    [SwaggerParameter(Description = "Unit price of the product")] decimal Price,
    [SwaggerParameter(Description = "Current stock quantity")] int Stock,
    [SwaggerParameter(Description = "Minimum stock threshold before a low stock alert is raised")] int MinimumStockThreshold,
    [SwaggerParameter(Description = "Date of the last restock (YYYY-MM-DD)")] string LastRestockDate);
