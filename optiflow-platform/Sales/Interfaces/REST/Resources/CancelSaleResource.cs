using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.Sales.Interfaces.REST.Resources;

[SwaggerSchema(Description = "Payload to cancel a sale")]
public record CancelSaleResource(
    [Required]
    [SwaggerParameter(Description = "Current lab order status (e.g. IN_PRODUCTION, READY, DELIVERED)")]
    string LabOrderStatus);
