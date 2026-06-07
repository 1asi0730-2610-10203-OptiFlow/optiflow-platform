using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.Inventory.Interfaces.REST.Resources;

/// <summary>
///     Request payload to register a new product in the catalog.
/// </summary>
[SwaggerSchema(Description = "Request payload to register a product")]
public record RegisterProductResource(
    [SwaggerParameter(Description = "Reference to the product category")] int CategoryId,
    [Required]
    [SwaggerParameter(Description = "Name of the product category")] string Category,
    [SwaggerParameter(Description = "Reference to the supplier")] int SupplierId,
    [Required]
    [SwaggerParameter(Description = "Name of the supplier")] string SupplierName,
    [Required]
    [SwaggerParameter(Description = "Stock keeping unit code, must be unique")] string Sku,
    [Required]
    [SwaggerParameter(Description = "Name of the product")] string Name,
    [SwaggerParameter(Description = "Brand of the product")] string Brand,
    [SwaggerParameter(Description = "Model of the product")] string Model,
    [SwaggerParameter(Description = "Unit price of the product")] decimal Price,
    [SwaggerParameter(Description = "Initial stock quantity")] int Stock,
    [SwaggerParameter(Description = "Minimum stock threshold before a low stock alert is raised")] int MinimumStockThreshold);
