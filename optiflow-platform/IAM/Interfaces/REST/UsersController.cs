using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using optiflow_platform.IAM.Application.CommandServices;
using optiflow_platform.IAM.Application.QueryServices;
using optiflow_platform.IAM.Domain.Model.Queries;
using optiflow_platform.IAM.Domain.Model.ValueObjects;
using optiflow_platform.IAM.Interfaces.REST.Resources;
using optiflow_platform.IAM.Interfaces.REST.Transform;
using optiflow_platform.IAM.Infrastructure.Pipeline.Middleware.Attributes;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.IAM.Interfaces.REST;

[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Users")]
[Authorize]
[AllowWithoutSubscription]
public class UsersController(
    IUserCommandService userCommandService,
    IUserQueryService userQueryService,
    Microsoft.Extensions.Localization.IStringLocalizer<optiflow_platform.IAM.Resources.IamMessages> localizer,
    optiflow_platform.Shared.Interfaces.Rest.ProblemDetails.ProblemDetailsFactory problemDetailsFactory) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all users",
        Description = "Gets all registered users",
        OperationId = "GetAllUsers")]
    [SwaggerResponse(200, "List of users", typeof(IEnumerable<UserResource>))]
    public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
    {
        var query = new GetAllUsersQuery();
        var users = await userQueryService.Handle(query, cancellationToken);
        var resources = users.Select(UserResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery(new UserId(id));
        var user = await userQueryService.Handle(query, cancellationToken);

        if (user == null) return NotFound();

        var resource = UserResourceFromEntityAssembler.ToResourceFromEntity(user);
        return Ok(resource);
    }

    [HttpPut("{id}/email")]
    public async Task<IActionResult> UpdateUserEmail(Guid id, [FromBody] UpdateUserEmailResource resource, CancellationToken cancellationToken)
    {
        var command = UpdateUserEmailCommandFromResourceAssembler.ToCommandFromResource(id, resource);
        var result = await userCommandService.Handle(command, cancellationToken);

        if (!result.IsSuccess)
            return IamErrorToActionAssembler.ToActionResult(result.Error, result.Message, this, problemDetailsFactory, localizer);

        var authenticatedUserResource = AuthenticatedUserResourceFromEntityAssembler.ToResourceFromEntity(result.Value!.User, result.Value.Token);
        return Ok(authenticatedUserResource);
    }

    [HttpPut("{id}/password")]
    public async Task<IActionResult> UpdateUserPassword(Guid id, [FromBody] UpdateUserPasswordResource resource, CancellationToken cancellationToken)
    {
        var command = UpdateUserPasswordCommandFromResourceAssembler.ToCommandFromResource(id, resource);
        var result = await userCommandService.Handle(command, cancellationToken);

        if (!result.IsSuccess)
            return IamErrorToActionAssembler.ToActionResult(result.Error, result.Message, this, problemDetailsFactory, localizer);

        return Ok(new { Message = "Password updated successfully." });
    }
}
