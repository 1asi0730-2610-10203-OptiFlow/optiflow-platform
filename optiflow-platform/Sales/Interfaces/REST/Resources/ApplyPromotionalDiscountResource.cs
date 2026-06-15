using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.Sales.Interfaces.REST.Resources;

[SwaggerSchema(Description = "Payload to apply a promotional discount")]
public record ApplyPromotionalDiscountResource(
    [Required] [SwaggerParameter(Description = "Discount code to apply")] string DiscountCode,
    [Required] [SwaggerParameter(Description = "Fixed discount amount to subtract from total")] decimal DiscountAmount);
