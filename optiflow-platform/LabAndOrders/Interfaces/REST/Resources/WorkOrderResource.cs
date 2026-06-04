using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.LabAndOrders.Interfaces.REST.Resources;

/// <summary>
///     Represents the data provided by the server about a work order.
/// </summary>
[SwaggerSchema(Description = "A work order resource")]
public record WorkOrderResource(
    [SwaggerParameter(Description = "The server-generated ID of the work order")] int Id,
    [SwaggerParameter(Description = "Reference to the related sale")] int SaleId,
    [SwaggerParameter(Description = "Reference to the optical prescription")] int RecipeId,
    [SwaggerParameter(Description = "Reference to the laboratory")] int LabId,
    [SwaggerParameter(Description = "Current status of the work order")] string Status,
    [SwaggerParameter(Description = "Priority of the work order")] string Priority,
    [SwaggerParameter(Description = "Full name of the patient")] string PatientName,
    [SwaggerParameter(Description = "Name of the laboratory")] string LaboratoryName,
    [SwaggerParameter(Description = "Type of lenses")] string LensType,
    [SwaggerParameter(Description = "Frame description")] string Frame,
    [SwaggerParameter(Description = "Optical prescription summary")] string Prescription,
    [SwaggerParameter(Description = "Expected delivery date")] string DeliveryDate,
    [SwaggerParameter(Description = "Initial deposit amount")] decimal Deposit,
    [SwaggerParameter(Description = "Total order amount")] decimal Total,
    [SwaggerParameter(Description = "Indicates if this is a rework order")] bool IsRework);
