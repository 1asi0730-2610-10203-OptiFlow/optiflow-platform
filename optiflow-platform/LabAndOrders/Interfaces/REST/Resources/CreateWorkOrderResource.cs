using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.LabAndOrders.Interfaces.REST.Resources;

/// <summary>
///     Request payload to create a new work order.
/// </summary>
[SwaggerSchema(Description = "Request payload to create a work order")]
public record CreateWorkOrderResource(
    [SwaggerParameter(Description = "Reference to the related sale")] int SaleId,
    [SwaggerParameter(Description = "Reference to the optical prescription")] int RecipeId,
    [SwaggerParameter(Description = "Reference to the laboratory")] int LabId,
    [Required]
    [StringLength(100, ErrorMessage = "PatientName must be at most 100 characters")]
    [SwaggerParameter(Description = "Full name of the patient")] string PatientName,
    [StringLength(100, ErrorMessage = "LaboratoryName must be at most 100 characters")]
    [SwaggerParameter(Description = "Name of the laboratory")] string LaboratoryName,
    [StringLength(50, ErrorMessage = "LensType must be at most 50 characters")]
    [SwaggerParameter(Description = "Type of lenses (e.g. Progresivas, Bifocales)")] string LensType,
    [SwaggerParameter(Description = "Reference to the Inventory product consumed as lens material")] int? LensProductId,
    [StringLength(100, ErrorMessage = "Frame must be at most 100 characters")]
    [SwaggerParameter(Description = "Frame description")] string Frame,
    [SwaggerParameter(Description = "Reference to the Inventory product consumed as frame material")] int? FrameProductId,
    [StringLength(500, ErrorMessage = "Prescription must be at most 500 characters")]
    [SwaggerParameter(Description = "Optical prescription summary")] string Prescription,
    [StringLength(20, ErrorMessage = "Priority must be at most 20 characters")]
    [SwaggerParameter(Description = "Order priority: normal, high, urgent")] string Priority,
    [Required]
    [SwaggerParameter(Description = "Expected delivery date (YYYY-MM-DD)")] string DeliveryDate,
    [Range(0, 1000000, ErrorMessage = "Deposit must be between 0 and 1000000")]
    [SwaggerParameter(Description = "Initial deposit amount")] decimal Deposit,
    [Range(0, 1000000, ErrorMessage = "Total must be between 0 and 1000000")]
    [SwaggerParameter(Description = "Total order amount")] decimal Total);
