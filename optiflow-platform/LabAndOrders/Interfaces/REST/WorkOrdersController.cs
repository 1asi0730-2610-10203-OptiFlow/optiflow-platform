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
using optiflow_platform.IAM.Infrastructure.Pipeline.Middleware.Attributes;

namespace optiflow_platform.LabAndOrders.Interfaces.REST;

/// <summary>
///     Work orders controller.
/// </summary>
[ApiController]
[Route("/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Work Orders")]
[Authorize]
public class WorkOrdersController(
    IWorkOrderCommandService workOrderCommandService,
    IWorkOrderQueryService workOrderQueryService,
    ILogger<WorkOrdersController> logger)
    : ControllerBase
{
    /// <summary>
    ///     Creates a new work order.
    /// </summary>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Creates a work order",
        Description = "Creates a new work order with PENDING status for the given patient and laboratory",
        OperationId = "CreateWorkOrder")]
    [SwaggerResponse(201, "The work order was created", typeof(WorkOrderResource))]
    [SwaggerResponse(400, "The request payload is invalid", typeof(string))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> CreateWorkOrder([FromBody] CreateWorkOrderResource resource,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = CreateWorkOrderCommandFromResourceAssembler.ToCommandFromResource(resource);
            var result = await workOrderCommandService.Handle(command, cancellationToken);
            return result switch
            {
                Result<WorkOrder, CreateWorkOrderError>.Success success =>
                    CreatedAtAction(nameof(GetWorkOrderById),
                        new { id = success.Value.Id },
                        WorkOrderResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
                Result<WorkOrder, CreateWorkOrderError>.Failure =>
                    Problem(title: "Unexpected error", detail: "Could not create work order.", statusCode: 500),
                _ => Problem(statusCode: 500)
            };
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Validation failed while creating work order for patient {PatientName}",
                resource.PatientName);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while creating work order for patient {PatientName}",
                resource.PatientName);
            return Problem(title: "Unexpected server error",
                detail: "An unexpected error occurred while creating the work order.", statusCode: 500);
        }
    }

    /// <summary>
    ///     Gets all work orders.
    /// </summary>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Gets all work orders",
        Description = "Returns all work orders in the system",
        OperationId = "GetAllWorkOrders")]
    [SwaggerResponse(200, "List of work orders", typeof(IEnumerable<WorkOrderResource>))]
    public async Task<ActionResult> GetAllWorkOrders(CancellationToken cancellationToken = default)
    {
        var query = new GetAllWorkOrdersQuery();
        var result = await workOrderQueryService.Handle(query, cancellationToken);
        var resources = result.Select(WorkOrderResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    /// <summary>
    ///     Gets a work order by id.
    /// </summary>
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Gets a work order by id",
        Description = "Returns a single work order for the given identifier",
        OperationId = "GetWorkOrderById")]
    [SwaggerResponse(200, "The work order was found", typeof(WorkOrderResource))]
    [SwaggerResponse(404, "The work order was not found")]
    public async Task<ActionResult> GetWorkOrderById(int id, CancellationToken cancellationToken = default)
    {
        var query = new GetWorkOrderByIdQuery(id);
        var result = await workOrderQueryService.Handle(query, cancellationToken);
        if (result is null) return NotFound();
        return Ok(WorkOrderResourceFromEntityAssembler.ToResourceFromEntity(result));
    }

    /// <summary>
    ///     Updates the status of a work order.
    /// </summary>
    [HttpPatch("{id}/status")]
    [SwaggerOperation(
        Summary = "Updates a work order status",
        Description = "Transitions a work order to a new status. Backward transitions are flagged as rework.",
        OperationId = "UpdateWorkOrderStatus")]
    [SwaggerResponse(200, "The status was updated", typeof(WorkOrderResource))]
    [SwaggerResponse(400, "The provided status is invalid", typeof(string))]
    [SwaggerResponse(404, "The work order was not found")]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> UpdateWorkOrderStatus(int id,
        [FromBody] UpdateWorkOrderStatusResource resource,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = UpdateOrderStatusCommandFromResourceAssembler.ToCommandFromResource(id, resource);
            var result = await workOrderCommandService.Handle(command, cancellationToken);
            return result switch
            {
                Result<WorkOrder, UpdateOrderStatusError>.Success success =>
                    Ok(WorkOrderResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
                Result<WorkOrder, UpdateOrderStatusError>.Failure { Error: UpdateOrderStatusError.WorkOrderNotFound } =>
                    NotFound(),
                Result<WorkOrder, UpdateOrderStatusError>.Failure { Error: UpdateOrderStatusError.InvalidStatus } =>
                    BadRequest($"Invalid status value: {resource.Status}"),
                _ => Problem(title: "Unexpected server error",
                    detail: "Could not update work order status.", statusCode: 500)
            };
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Validation failed while updating work order {WorkOrderId} status", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while updating work order {WorkOrderId} status", id);
            return Problem(title: "Unexpected server error",
                detail: "An unexpected error occurred while updating the work order status.", statusCode: 500);
        }
    }
}
