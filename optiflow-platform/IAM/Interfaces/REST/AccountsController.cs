using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using optiflow_platform.IAM.Application.CommandServices;
using optiflow_platform.IAM.Application.QueryServices;
using optiflow_platform.IAM.Domain.Model.Queries;
using optiflow_platform.IAM.Infrastructure.Pipeline.Middleware.Attributes;
using optiflow_platform.IAM.Interfaces.REST.Resources;
using optiflow_platform.IAM.Interfaces.REST.Transform;
using optiflow_platform.Shared.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.IAM.Interfaces.REST;

[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Accounts")]
[Authorize]
public class AccountsController(
    IAccountCommandService accountCommandService,
    IAccountQueryService accountQueryService,
    ICurrentUserContext currentUserContext,
    Microsoft.Extensions.Localization.IStringLocalizer<optiflow_platform.IAM.Resources.IamMessages> localizer,
    optiflow_platform.Shared.Interfaces.Rest.ProblemDetails.ProblemDetailsFactory problemDetailsFactory) : ControllerBase
{
    /// <summary>Crea la cuenta (negocio) del usuario autenticado, completando el onboarding.</summary>
    [HttpPost]
    [AllowWithoutAccount]
    [SwaggerOperation(Summary = "Create my account", OperationId = "CreateAccount")]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountResource resource, CancellationToken cancellationToken)
    {
        if (currentUserContext.UserId is not { } userId)
            return Unauthorized();

        var command = CreateAccountCommandFromResourceAssembler.ToCommandFromResource(resource, userId);
        var result = await accountCommandService.Handle(command, cancellationToken);

        if (!result.IsSuccess)
            return IamErrorToActionAssembler.ToActionResult(result.Error, result.Message, this, problemDetailsFactory, localizer);

        return Ok(AccountResourceFromEntityAssembler.ToResourceFromEntity(result.Value!));
    }

    /// <summary>Obtiene la cuenta del usuario autenticado, si ya completó el onboarding.</summary>
    [HttpGet("me")]
    [AllowWithoutAccount]
    [SwaggerOperation(Summary = "Get my account", OperationId = "GetMyAccount")]
    public async Task<IActionResult> GetMyAccount(CancellationToken cancellationToken)
    {
        if (currentUserContext.AccountId is not { } accountId)
            return NoContent();

        var account = await accountQueryService.Handle(new GetAccountByIdQuery(accountId), cancellationToken);
        if (account == null)
            return NoContent();

        return Ok(AccountResourceFromEntityAssembler.ToResourceFromEntity(account));
    }
}
