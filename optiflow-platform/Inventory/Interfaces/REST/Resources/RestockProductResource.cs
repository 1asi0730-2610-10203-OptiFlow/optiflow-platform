using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.Inventory.Interfaces.REST.Resources;

/// <summary>
///     Request payload to restock a product.
/// </summary>
[SwaggerSchema(Description = "Request payload to restock a product")]
public record RestockProductResource(
    [SwaggerParameter(Description = "Quantity of units received, must be greater than zero")] int Quantity,
    [Required]
    [SwaggerParameter(Description = "Name of the technical performing the restock")] string Author);
