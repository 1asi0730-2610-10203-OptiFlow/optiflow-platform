using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.Inventory.Interfaces.REST.Resources;

/// <summary>
///     Request payload to restock a product.
/// </summary>
[SwaggerSchema(Description = "Request payload to restock a product")]
public record RestockProductResource(
    [Range(1, 1000000, ErrorMessage = "Quantity must be between 1 and 1000000")]
    [SwaggerParameter(Description = "Quantity of units received, must be greater than zero")] int Quantity,
    [Required]
    [StringLength(100, ErrorMessage = "Author must be at most 100 characters")]
    [SwaggerParameter(Description = "Name of the technical performing the restock")] string Author);
