using System.Net.Mime;
using optiflow_platform.LabAndOrders.Application.Services;
using optiflow_platform.LabAndOrders.Domain.Model.Queries;
using optiflow_platform.LabAndOrders.Interfaces.REST.Resources;
using optiflow_platform.LabAndOrders.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.LabAndOrders.Interfaces.REST;

/// <summary>
///     Laboratories controller.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Laboratories")]
public class LaboratoriesController(ILaboratoryQueryService laboratoryQueryService) : ControllerBase
{
    /// <summary>
    ///     Gets all laboratories.
    /// </summary>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Gets all laboratories",
        Description = "Returns all registered optical laboratories",
        OperationId = "GetAllLaboratories")]
    [SwaggerResponse(200, "List of laboratories", typeof(IEnumerable<LaboratoryResource>))]
    public async Task<ActionResult> GetAllLaboratories(CancellationToken cancellationToken = default)
    {
        var query = new GetAllLaboratoriesQuery();
        var result = await laboratoryQueryService.Handle(query, cancellationToken);
        var resources = result.Select(LaboratoryResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    /// <summary>
    ///     Gets a laboratory by id.
    /// </summary>
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Gets a laboratory by id",
        Description = "Returns a single laboratory for the given identifier",
        OperationId = "GetLaboratoryById")]
    [SwaggerResponse(200, "The laboratory was found", typeof(LaboratoryResource))]
    [SwaggerResponse(404, "The laboratory was not found")]
    public async Task<ActionResult> GetLaboratoryById(int id, CancellationToken cancellationToken = default)
    {
        var query = new GetLaboratoryByIdQuery(id);
        var result = await laboratoryQueryService.Handle(query, cancellationToken);
        if (result is null) return NotFound();
        return Ok(LaboratoryResourceFromEntityAssembler.ToResourceFromEntity(result));
    }
}
