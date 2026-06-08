using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.Sales.Interfaces.REST.Resources;

[SwaggerSchema(Description = "Payload to apply a promotional discount")]
public record ApplyPromotionalDiscountResource(
    [Required] [SwaggerParameter(Description = "Discount percentage to apply (0–100)")] decimal DiscountPercentage);
