using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.Inventory.Interfaces.REST.Resources;

/// <summary>
///     Request payload to register a new supplier.
/// </summary>
[SwaggerSchema(Description = "Request payload to register a supplier")]
public record RegisterSupplierResource(
    [Required]
    [StringLength(100, ErrorMessage = "Name must be at most 100 characters")]
    [SwaggerParameter(Description = "Unique name that identifies the supplier")] string Name,
    [Required]
    [StringLength(100, ErrorMessage = "ContactPerson must be at most 100 characters")]
    [SwaggerParameter(Description = "Name of the contact person at the supplier")] string ContactPerson,
    [Required]
    [RegularExpression(@"^\+?[0-9\s()\-.]{7,20}$", ErrorMessage = "Phone must be 7-20 characters and may include +, spaces, dashes or parentheses")]
    [SwaggerParameter(Description = "Contact phone number of the supplier")] string Phone,
    [Required]
    [EmailAddress(ErrorMessage = "Email must be a valid email address")]
    [StringLength(100, ErrorMessage = "Email must be at most 100 characters")]
    [SwaggerParameter(Description = "Contact email address of the supplier")] string Email);
