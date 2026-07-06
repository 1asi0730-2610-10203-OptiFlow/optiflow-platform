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
[Tags("Roles")]
[Authorize]
public class RolesController(
    IRoleCommandService roleCommandService,
    IRoleQueryService roleQueryService,
    ILogger<RolesController> logger) : ControllerBase
{
    /// <summary>Gets all roles of the current optic.</summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Gets all roles", OperationId = "GetAllRoles")]
    public async Task<ActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        var result = await roleQueryService.Handle(new GetAllRolesQuery(), cancellationToken);
        return Ok(result.Select(RoleResourceFromEntityAssembler.ToResourceFromEntity));
    }

    /// <summary>Gets a role by id.</summary>
    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Gets a role by id", OperationId = "GetRoleById")]
    public async Task<ActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        var role = await roleQueryService.Handle(new GetRoleByIdQuery(id), cancellationToken);
        if (role is null) return NotFound();
        return Ok(RoleResourceFromEntityAssembler.ToResourceFromEntity(role));
    }

    /// <summary>Creates a role.</summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Creates a role", OperationId = "CreateRole")]
    public async Task<ActionResult> Create([FromBody] SaveRoleResource resource, CancellationToken cancellationToken)
    {
        try
        {
            var command = CreateRoleCommandFromResourceAssembler.ToCommandFromResource(resource);
            var role = await roleCommandService.Handle(command, cancellationToken);
            if (role is null) return Problem(title: "Could not create role", statusCode: 500);
            return CreatedAtAction(nameof(GetById), new { id = role.Id },
                RoleResourceFromEntityAssembler.ToResourceFromEntity(role));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating role");
            return Problem(title: "Unexpected error", detail: "Could not create role.", statusCode: 500);
        }
    }

    /// <summary>Updates a role.</summary>
    [HttpPut("{id:int}")]
    [SwaggerOperation(Summary = "Updates a role", OperationId = "UpdateRole")]
    public async Task<ActionResult> Update(int id, [FromBody] SaveRoleResource resource, CancellationToken cancellationToken)
    {
        var command = UpdateRoleCommandFromResourceAssembler.ToCommandFromResource(id, resource);
        var role = await roleCommandService.Handle(command, cancellationToken);
        if (role is null) return NotFound();
        return Ok(RoleResourceFromEntityAssembler.ToResourceFromEntity(role));
    }

    /// <summary>Deletes a role.</summary>
    [HttpDelete("{id:int}")]
    [SwaggerOperation(Summary = "Deletes a role", OperationId = "DeleteRole")]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await roleCommandService.DeleteAsync(id, cancellationToken);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
