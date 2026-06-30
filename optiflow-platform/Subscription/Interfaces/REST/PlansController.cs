using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using optiflow_platform.Shared.Application.Patterns;
using optiflow_platform.Subscription.Application.Errors;
using optiflow_platform.Subscription.Application.Services;
using optiflow_platform.Subscription.Domain.Model.Aggregates;
using optiflow_platform.Subscription.Domain.Model.Queries;
using optiflow_platform.Subscription.Domain.Model.ValueObjects;
using optiflow_platform.Subscription.Interfaces.REST.Resources;
using optiflow_platform.Subscription.Interfaces.REST.Transform;
using Swashbuckle.AspNetCore.Annotations;
using optiflow_platform.IAM.Infrastructure.Pipeline.Middleware.Attributes;

namespace optiflow_platform.Subscription.Interfaces.REST;

[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Plans")]
[Authorize]
public class PlansController(
    IPlanCommandService planCommandService,
    IPlanQueryService planQueryService,
    ILogger<PlansController> logger)
    : ControllerBase
{
    /// <summary>Creates a new subscription plan.</summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Creates a plan", OperationId = "CreatePlan")]
    [SwaggerResponse(201, "Plan created", typeof(PlanResource))]
    [SwaggerResponse(400, "Invalid request payload or name already exists", typeof(string))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> CreatePlan([FromBody] CreatePlanResource resource, CancellationToken cancellationToken)
    {
        try
        {
            var command = CreatePlanCommandFromResourceAssembler.ToCommandFromResource(resource);
            var result = await planCommandService.Handle(command, cancellationToken);
            return result switch
            {
                Result<Plan, CreatePlanError>.Success success =>
                    CreatedAtAction(nameof(GetPlanById),
                        new { id = success.Value.Id },
                        PlanResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
                Result<Plan, CreatePlanError>.Failure { Error: CreatePlanError.NameAlreadyExists } =>
                    BadRequest($"A plan named '{resource.Name}' already exists."),
                _ => Problem(title: "Unexpected error", detail: "Could not create plan.", statusCode: 500)
            };
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Validation failed creating plan '{Name}'", resource.Name);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error creating plan '{Name}'", resource.Name);
            return Problem(title: "Unexpected server error", detail: "Could not create plan.", statusCode: 500);
        }
    }

    /// <summary>Gets all plans.</summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Gets all plans", OperationId = "GetAllPlans")]
    [SwaggerResponse(200, "List of plans", typeof(IEnumerable<PlanResource>))]
    public async Task<ActionResult> GetAllPlans(CancellationToken cancellationToken = default)
    {
        var result = await planQueryService.Handle(new GetAllPlansQuery(), cancellationToken);
        return Ok(result.Select(PlanResourceFromEntityAssembler.ToResourceFromEntity));
    }

    /// <summary>Gets a plan by id.</summary>
    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Gets a plan by id", OperationId = "GetPlanById")]
    [SwaggerResponse(200, "The plan was found", typeof(PlanResource))]
    [SwaggerResponse(404, "Plan not found")]
    public async Task<ActionResult> GetPlanById(int id, CancellationToken cancellationToken = default)
    {
        var result = await planQueryService.Handle(new GetPlanByIdQuery(new PlanId(id)), cancellationToken);
        if (result is null) return NotFound();
        return Ok(PlanResourceFromEntityAssembler.ToResourceFromEntity(result));
    }
}
