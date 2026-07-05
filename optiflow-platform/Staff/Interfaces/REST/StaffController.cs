using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using optiflow_platform.IAM.Infrastructure.Pipeline.Middleware.Attributes;
using optiflow_platform.Staff.Application.Services;
using optiflow_platform.Staff.Domain.Model.Queries;
using optiflow_platform.Staff.Interfaces.REST.Resources;
using optiflow_platform.Staff.Interfaces.REST.Transform;
using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.Staff.Interfaces.REST;

[ApiController]
[Route("/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Staff")]
[Authorize]
public class StaffController(
    IStaffCommandService staffCommandService,
    IStaffQueryService staffQueryService,
    ILogger<StaffController> logger) : ControllerBase
{
    /// <summary>Gets all staff members of the current optic.</summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Gets all staff members", OperationId = "GetAllStaff")]
    public async Task<ActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        var result = await staffQueryService.Handle(new GetAllStaffQuery(), cancellationToken);
        return Ok(result.Select(StaffResourceFromEntityAssembler.ToResourceFromEntity));
    }

    /// <summary>Gets a staff member by id.</summary>
    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Gets a staff member by id", OperationId = "GetStaffById")]
    public async Task<ActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        var staff = await staffQueryService.Handle(new GetStaffByIdQuery(id), cancellationToken);
        if (staff is null) return NotFound();
        return Ok(StaffResourceFromEntityAssembler.ToResourceFromEntity(staff));
    }

    /// <summary>Registers a new staff member.</summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Creates a staff member", OperationId = "CreateStaff")]
    public async Task<ActionResult> Create([FromBody] SaveStaffResource resource, CancellationToken cancellationToken)
    {
        try
        {
            var command = CreateStaffCommandFromResourceAssembler.ToCommandFromResource(resource);
            var staff = await staffCommandService.Handle(command, cancellationToken);
            if (staff is null) return Problem(title: "Could not create staff member", statusCode: 500);
            return CreatedAtAction(nameof(GetById), new { id = staff.Id },
                StaffResourceFromEntityAssembler.ToResourceFromEntity(staff));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating staff member");
            return Problem(title: "Unexpected error", detail: "Could not create staff member.", statusCode: 500);
        }
    }

    /// <summary>Updates an existing staff member.</summary>
    [HttpPut("{id:int}")]
    [SwaggerOperation(Summary = "Updates a staff member", OperationId = "UpdateStaff")]
    public async Task<ActionResult> Update(int id, [FromBody] SaveStaffResource resource, CancellationToken cancellationToken)
    {
        var command = UpdateStaffCommandFromResourceAssembler.ToCommandFromResource(id, resource);
        var staff = await staffCommandService.Handle(command, cancellationToken);
        if (staff is null) return NotFound();
        return Ok(StaffResourceFromEntityAssembler.ToResourceFromEntity(staff));
    }

    /// <summary>Deletes a staff member.</summary>
    [HttpDelete("{id:int}")]
    [SwaggerOperation(Summary = "Deletes a staff member", OperationId = "DeleteStaff")]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await staffCommandService.DeleteAsync(id, cancellationToken);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
