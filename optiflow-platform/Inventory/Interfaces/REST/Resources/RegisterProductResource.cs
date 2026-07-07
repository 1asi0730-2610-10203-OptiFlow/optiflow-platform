using System.ComponentModel.DataAnnotations;
using optiflow_platform.Inventory.Domain.Model.ValueObjects;
using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.Inventory.Interfaces.REST.Resources;

/// <summary>
///     Request payload to register a new product in the catalog.
/// </summary>
[SwaggerSchema(Description = "Request payload to register a product")]
public record RegisterProductResource(
    [Required]
    [SwaggerParameter(Description = "The product category")] EProductCategory Category,
    [Range(1, int.MaxValue, ErrorMessage = "SupplierId must be a positive identifier")]
    [SwaggerParameter(Description = "Reference to the supplier")] int SupplierId,
    [Required]
    [SwaggerParameter(Description = "Name of the supplier")] string SupplierName,
    [Required]
    [SwaggerParameter(Description = "Stock keeping unit code, must be unique")] string Sku,
    [Required]
    [SwaggerParameter(Description = "Name of the product")] string Name,
    [SwaggerParameter(Description = "Brand of the product")] string Brand,
    [SwaggerParameter(Description = "Model of the product")] string Model,
    [Range(0.01, 1000000, ErrorMessage = "Price must be between 0.01 and 1000000")]
    [SwaggerParameter(Description = "Unit price of the product")] decimal Price,
    [Range(0, 1000000, ErrorMessage = "Stock must be between 0 and 1000000")]
    [SwaggerParameter(Description = "Initial stock quantity")] int Stock,
    [Range(0, 1000000, ErrorMessage = "MinimumStockThreshold must be between 0 and 1000000")]
    [SwaggerParameter(Description = "Minimum stock threshold before a low stock alert is raised")] int MinimumStockThreshold);
