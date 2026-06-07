using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.Inventory.Interfaces.REST.Resources;

/// <summary>
///     Represents the data provided by the server about a supplier.
/// </summary>
[SwaggerSchema(Description = "A supplier resource")]
public record SupplierResource(
    [SwaggerParameter(Description = "The server-generated ID of the supplier")] int Id,
    [SwaggerParameter(Description = "Name of the supplier")] string Name,
    [SwaggerParameter(Description = "Name of the contact person at the supplier")] string ContactPerson,
    [SwaggerParameter(Description = "Contact phone number")] string Phone,
    [SwaggerParameter(Description = "Contact email address")] string Email);
