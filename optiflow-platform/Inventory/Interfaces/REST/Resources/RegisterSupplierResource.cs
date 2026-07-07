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
    [StringLength(50, ErrorMessage = "Phone must be at most 50 characters")]
    [RegularExpression(@"^\+?([\s().-]*\d){7,15}[\s().-]*$", ErrorMessage = "Phone must contain 7 to 15 digits and may include +, spaces, dashes or parentheses")]
    [SwaggerParameter(Description = "Contact phone number of the supplier")] string Phone,
    [Required]
    [EmailAddress(ErrorMessage = "Email must be a valid email address")]
    [StringLength(100, ErrorMessage = "Email must be at most 100 characters")]
    [SwaggerParameter(Description = "Contact email address of the supplier")] string Email);
