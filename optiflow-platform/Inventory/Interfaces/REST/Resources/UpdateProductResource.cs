using System.ComponentModel.DataAnnotations;
using optiflow_platform.Inventory.Domain.Model.ValueObjects;
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
    [SwaggerParameter(Description = "The product category")] EProductCategory Category,
    [Range(0.01, 1000000, ErrorMessage = "Price must be between 0.01 and 1000000")]
    [SwaggerParameter(Description = "Unit price of the product")] decimal Price,
    [Range(0, 1000000, ErrorMessage = "MinimumStockThreshold must be between 0 and 1000000")]
    [SwaggerParameter(Description = "Minimum stock threshold before a low stock alert is raised")] int MinimumStockThreshold);
