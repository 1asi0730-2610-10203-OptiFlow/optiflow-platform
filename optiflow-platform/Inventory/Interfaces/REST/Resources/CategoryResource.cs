using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.Inventory.Interfaces.REST.Resources;

/// <summary>
///     Represents the data provided by the server about a product category.
/// </summary>
[SwaggerSchema(Description = "A product category resource")]
public record CategoryResource(
    [SwaggerParameter(Description = "The server-generated ID of the category")] int Id,
    [SwaggerParameter(Description = "Name of the category")] string Name);
