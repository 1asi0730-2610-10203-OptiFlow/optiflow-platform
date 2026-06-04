using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.LabAndOrders.Interfaces.REST.Resources;

/// <summary>
///     Request payload to update a work order status.
/// </summary>
[SwaggerSchema(Description = "Request payload to update a work order status")]
public record UpdateWorkOrderStatusResource(
    [Required]
    [SwaggerParameter(Description = "New status: PENDING, IN_PRODUCTION, QUALITY_CONTROL, READY, DELIVERED")] string Status);
