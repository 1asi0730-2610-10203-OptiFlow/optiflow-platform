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
    [SwaggerParameter(Description = "Full name of the patient")] string PatientName,
    [SwaggerParameter(Description = "Name of the laboratory")] string LaboratoryName,
    [SwaggerParameter(Description = "Type of lenses (e.g. Progresivas, Bifocales)")] string LensType,
    [SwaggerParameter(Description = "Reference to the Inventory product consumed as lens material")] int? LensProductId,
    [SwaggerParameter(Description = "Frame description")] string Frame,
    [SwaggerParameter(Description = "Reference to the Inventory product consumed as frame material")] int? FrameProductId,
    [SwaggerParameter(Description = "Optical prescription summary")] string Prescription,
    [SwaggerParameter(Description = "Order priority: normal, high, urgent")] string Priority,
    [Required]
    [SwaggerParameter(Description = "Expected delivery date (YYYY-MM-DD)")] string DeliveryDate,
    [SwaggerParameter(Description = "Initial deposit amount")] decimal Deposit,
    [SwaggerParameter(Description = "Total order amount")] decimal Total);
