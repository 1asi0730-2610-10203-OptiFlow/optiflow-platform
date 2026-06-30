using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using optiflow_platform.PatientCenter.Application.Services;
using optiflow_platform.PatientCenter.Domain.Model.Queries;
using optiflow_platform.PatientCenter.Interfaces.REST.Resources;
using optiflow_platform.PatientCenter.Interfaces.REST.Transform;
using Swashbuckle.AspNetCore.Annotations;
using optiflow_platform.IAM.Infrastructure.Pipeline.Middleware.Attributes;

namespace optiflow_platform.PatientCenter.Interfaces.REST;

[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Lens Materials")]
[Authorize]
public class LensMaterialsController(ILensMaterialQueryService lensMaterialQueryService)
    : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Gets all lens materials", OperationId = "GetAllLensMaterials")]
    [SwaggerResponse(200, "List of lens materials", typeof(IEnumerable<LensMaterialResource>))]
    public async Task<ActionResult> GetAllLensMaterials(CancellationToken cancellationToken = default)
    {
        var result    = await lensMaterialQueryService.Handle(new GetAllLensMaterialsQuery(), cancellationToken);
        var resources = result.Select(LensMaterialResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }
}