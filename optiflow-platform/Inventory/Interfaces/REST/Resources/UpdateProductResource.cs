using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.Inventory.Interfaces.REST.Resources;

/// <summary>
///     Request payload to update a product's catalog details.
/// </summary>
[SwaggerSchema(Description = "Request payload to update a product's catalog details")]
public record UpdateProductResource(
    [Required]
    [SwaggerParameter(Description = "Name of the product")] string Name,
    [Required]
    [SwaggerParameter(Description = "Stock keeping unit code")] string Sku,
    [Required]
    [SwaggerParameter(Description = "Name of the product category")] string Category,
    [SwaggerParameter(Description = "Unit price of the product")] decimal Price,
    [SwaggerParameter(Description = "Minimum stock threshold before a low stock alert is raised")] int MinimumStockThreshold);
