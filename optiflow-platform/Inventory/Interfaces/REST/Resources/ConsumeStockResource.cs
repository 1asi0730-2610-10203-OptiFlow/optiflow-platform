using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.Inventory.Interfaces.REST.Resources;

/// <summary>
///     Request payload to consume stock from a product.
/// </summary>
[SwaggerSchema(Description = "Request payload to consume product stock")]
public record ConsumeStockResource(
    [Range(1, 1000000, ErrorMessage = "Quantity must be between 1 and 1000000")]
    [SwaggerParameter(Description = "Quantity consumed")] int Quantity,
    [Required]
    [SwaggerParameter(Description = "Origin of the consumption, e.g. the work order that triggered it")] string Author);
