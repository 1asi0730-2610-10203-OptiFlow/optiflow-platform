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
    [StringLength(50, ErrorMessage = "Phone must be at most 50 characters")]
    [RegularExpression(@"^\+?([\s().-]*\d){7,15}[\s().-]*$", ErrorMessage = "Phone must contain 7 to 15 digits and may include +, spaces, dashes or parentheses")]
    [SwaggerParameter(Description = "Contact phone number of the laboratory")] string Phone,
    [Required]
    [EmailAddress(ErrorMessage = "Email must be a valid email address")]
    [StringLength(100, ErrorMessage = "Email must be at most 100 characters")]
    [SwaggerParameter(Description = "Contact email address of the laboratory")] string Email);
