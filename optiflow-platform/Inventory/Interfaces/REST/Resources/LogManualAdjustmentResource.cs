using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.Inventory.Interfaces.REST.Resources;

/// <summary>
///     Request payload to confirm a manual stock adjustment.
/// </summary>
[SwaggerSchema(Description = "Request payload to log a manual stock adjustment")]
public record LogManualAdjustmentResource(
    [Range(0, 1000000, ErrorMessage = "NewStock must be between 0 and 1000000")]
    [SwaggerParameter(Description = "New absolute stock quantity after the adjustment")] int NewStock,
    [Required]
    [SwaggerParameter(Description = "Justification for the stock adjustment")] string Justification,
    [Required]
    [SwaggerParameter(Description = "Name of the technical performing the adjustment")] string Author);
