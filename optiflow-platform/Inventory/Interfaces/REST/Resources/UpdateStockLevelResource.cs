using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.Inventory.Interfaces.REST.Resources;

/// <summary>
///     Request payload to set a product's stock to an absolute level.
/// </summary>
[SwaggerSchema(Description = "Request payload to update a product's stock level")]
public record UpdateStockLevelResource(
    [SwaggerParameter(Description = "New absolute stock quantity")] int NewStock);
