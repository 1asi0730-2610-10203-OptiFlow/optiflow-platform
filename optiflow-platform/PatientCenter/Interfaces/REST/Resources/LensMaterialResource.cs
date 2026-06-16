using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.PatientCenter.Interfaces.REST.Resources;

[SwaggerSchema(Description = "A lens material resource")]
public record LensMaterialResource(
    [SwaggerParameter("Material ID")]       int     Id,
    [SwaggerParameter("Full name")]         string  FullName,
    [SwaggerParameter("Refraction index")]  string  IndexValue,
    [SwaggerParameter("Base price")]        decimal BasePrice,
    [SwaggerParameter("Description")]       string  Description);