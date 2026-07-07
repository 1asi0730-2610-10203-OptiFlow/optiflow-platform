using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.LabAndOrders.Interfaces.REST.Resources;

/// <summary>
///     Request payload to register a new laboratory.
/// </summary>
[SwaggerSchema(Description = "Request payload to register a laboratory")]
public record CreateLaboratoryResource(
    [Required]
    [StringLength(100, ErrorMessage = "Name must be at most 100 characters")]
    [SwaggerParameter(Description = "Unique name that identifies the laboratory")] string Name,
    [Required]
    [RegularExpression(@"^\+?[0-9\s()\-.]{7,20}$", ErrorMessage = "Phone must be 7-20 characters and may include +, spaces, dashes or parentheses")]
    [SwaggerParameter(Description = "Contact phone number of the laboratory")] string Phone,
    [Required]
    [EmailAddress(ErrorMessage = "Email must be a valid email address")]
    [StringLength(100, ErrorMessage = "Email must be at most 100 characters")]
    [SwaggerParameter(Description = "Contact email address of the laboratory")] string Email);
