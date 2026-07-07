using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.Inventory.Interfaces.REST.Resources;

/// <summary>
///     Request payload to set a product's stock to an absolute level.
/// </summary>
[SwaggerSchema(Description = "Request payload to update a product's stock level")]
public record UpdateStockLevelResource(
    [Range(0, 1000000, ErrorMessage = "NewStock must be between 0 and 1000000")]
    [SwaggerParameter(Description = "New absolute stock quantity")] int NewStock);
