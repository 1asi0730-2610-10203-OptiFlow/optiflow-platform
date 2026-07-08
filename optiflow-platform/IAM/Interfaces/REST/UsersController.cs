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
    [SwaggerOperation(
        Summary = "Get a user by id",
        Description = "Gets a single registered user by its unique identifier",
        OperationId = "GetUserById")]
    [SwaggerResponse(200, "The user was found", typeof(UserResource))]
    [SwaggerResponse(404, "The user was not found")]
    public async Task<IActionResult> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery(new UserId(id));
        var user = await userQueryService.Handle(query, cancellationToken);

        if (user == null) return NotFound();

        var resource = UserResourceFromEntityAssembler.ToResourceFromEntity(user);
        return Ok(resource);
    }

    [HttpPut("{id}/email")]
    [SwaggerOperation(
        Summary = "Update a user's email",
        Description = "Changes the authenticated user's email and returns a refreshed authentication token",
        OperationId = "UpdateUserEmail")]
    [SwaggerResponse(200, "The email was updated", typeof(AuthenticatedUserResource))]
    [SwaggerResponse(400, "The request payload is invalid", typeof(ProblemDetails))]
    [SwaggerResponse(404, "The user was not found", typeof(ProblemDetails))]
    [SwaggerResponse(409, "The email is already in use", typeof(ProblemDetails))]
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
    [SwaggerOperation(
        Summary = "Update a user's password",
        Description = "Changes the authenticated user's password after validating the current one",
        OperationId = "UpdateUserPassword")]
    [SwaggerResponse(200, "The password was updated")]
    [SwaggerResponse(400, "The current password is incorrect or the payload is invalid", typeof(ProblemDetails))]
    [SwaggerResponse(404, "The user was not found", typeof(ProblemDetails))]
    public async Task<IActionResult> UpdateUserPassword(Guid id, [FromBody] UpdateUserPasswordResource resource, CancellationToken cancellationToken)
    {
        var command = UpdateUserPasswordCommandFromResourceAssembler.ToCommandFromResource(id, resource);
        var result = await userCommandService.Handle(command, cancellationToken);

        if (!result.IsSuccess)
            return IamErrorToActionAssembler.ToActionResult(result.Error, result.Message, this, problemDetailsFactory, localizer);

        return Ok(new { Message = "Password updated successfully." });
    }
}
