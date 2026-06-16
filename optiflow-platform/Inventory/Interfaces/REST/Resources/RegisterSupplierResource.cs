using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.Inventory.Interfaces.REST.Resources;

/// <summary>
///     Request payload to register a new supplier.
/// </summary>
[SwaggerSchema(Description = "Request payload to register a supplier")]
public record RegisterSupplierResource(
    [Required]
    [SwaggerParameter(Description = "Unique name that identifies the supplier")] string Name,
    [Required]
    [SwaggerParameter(Description = "Name of the contact person at the supplier")] string ContactPerson,
    [Required]
    [SwaggerParameter(Description = "Contact phone number of the supplier")] string Phone,
    [Required]
    [SwaggerParameter(Description = "Contact email address of the supplier")] string Email);
