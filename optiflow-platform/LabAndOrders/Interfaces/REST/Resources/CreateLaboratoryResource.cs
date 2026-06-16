using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.LabAndOrders.Interfaces.REST.Resources;

/// <summary>
///     Request payload to register a new laboratory.
/// </summary>
[SwaggerSchema(Description = "Request payload to register a laboratory")]
public record CreateLaboratoryResource(
    [Required]
    [SwaggerParameter(Description = "Unique name that identifies the laboratory")] string Name,
    [Required]
    [SwaggerParameter(Description = "Contact phone number of the laboratory")] string Phone,
    [Required]
    [SwaggerParameter(Description = "Contact email address of the laboratory")] string Email);
