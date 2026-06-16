using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.LabAndOrders.Interfaces.REST.Resources;

/// <summary>
///     Represents the data provided by the server about a laboratory.
/// </summary>
[SwaggerSchema(Description = "A laboratory resource")]
public record LaboratoryResource(
    [SwaggerParameter(Description = "The server-generated ID of the laboratory")] int Id,
    [SwaggerParameter(Description = "Name of the laboratory")] string Name,
    [SwaggerParameter(Description = "Contact phone number of the laboratory")] string Phone,
    [SwaggerParameter(Description = "Contact email address of the laboratory")] string Email);
