using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.LabAndOrders.Interfaces.REST.Resources;

/// <summary>
///     Request payload to link a work order to the sale created for it.
/// </summary>
[SwaggerSchema(Description = "Request payload to link a work order to a sale")]
public record LinkSaleResource(
    [Required]
    [SwaggerParameter(Description = "ID of the sale created for this work order")] int SaleId);
