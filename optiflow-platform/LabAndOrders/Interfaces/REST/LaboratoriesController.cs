using System.Net.Mime;
using optiflow_platform.LabAndOrders.Application.Errors;
using optiflow_platform.LabAndOrders.Application.Services;
using optiflow_platform.LabAndOrders.Domain.Model.Aggregates;
using optiflow_platform.LabAndOrders.Domain.Model.Queries;
using optiflow_platform.LabAndOrders.Interfaces.REST.Resources;
using optiflow_platform.LabAndOrders.Interfaces.REST.Transform;
using optiflow_platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.LabAndOrders.Interfaces.REST;

/// <summary>
///     Laboratories controller.
/// </summary>
[ApiController]
[Route("/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Laboratories")]
public class LaboratoriesController(
    ILaboratoryCommandService laboratoryCommandService,
    ILaboratoryQueryService laboratoryQueryService,
    ILogger<LaboratoriesController> logger) : ControllerBase
{
    /// <summary>
    ///     Registers a new laboratory.
    /// </summary>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Registers a laboratory",
        Description = "Creates a new optical laboratory with the given name and contact information",
        OperationId = "RegisterLaboratory")]
    [SwaggerResponse(201, "The laboratory was registered", typeof(LaboratoryResource))]
    [SwaggerResponse(400, "The request payload is invalid or the name is already taken", typeof(string))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> RegisterLaboratory([FromBody] CreateLaboratoryResource resource,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = CreateLaboratoryCommandFromResourceAssembler.ToCommandFromResource(resource);
            var result = await laboratoryCommandService.Handle(command, cancellationToken);
            return result switch
            {
                Result<Laboratory, CreateLaboratoryError>.Success success =>
                    CreatedAtAction(nameof(GetLaboratoryById),
                        new { id = success.Value.Id },
                        LaboratoryResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
                Result<Laboratory, CreateLaboratoryError>.Failure { Error: CreateLaboratoryError.DuplicateName } =>
                    BadRequest($"A laboratory named '{resource.Name}' already exists."),
                Result<Laboratory, CreateLaboratoryError>.Failure { Error: CreateLaboratoryError.InvalidContactInfo } =>
                    BadRequest("The provided phone or email is invalid."),
                _ => Problem(title: "Unexpected error", detail: "Could not register the laboratory.", statusCode: 500)
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while registering laboratory {Name}", resource.Name);
            return Problem(title: "Unexpected server error",
                detail: "An unexpected error occurred while registering the laboratory.", statusCode: 500);
        }
    }

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
