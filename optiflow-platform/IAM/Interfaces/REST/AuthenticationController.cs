using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using atelier_platform_aplicaciones_web.IAM.Application.CommandServices;
using atelier_platform_aplicaciones_web.IAM.Interfaces.REST.Resources;
using atelier_platform_aplicaciones_web.IAM.Interfaces.REST.Transform;
using atelier_platform_aplicaciones_web.IAM.Infrastructure.Pipeline.Middleware.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace atelier_platform_aplicaciones_web.IAM.Interfaces.REST;

[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Authentication")]
[AllowAnonymous]
public class AuthenticationController(
    IUserCommandService userCommandService,
    IPasswordRecoveryCommandService passwordRecoveryCommandService,
    Microsoft.Extensions.Localization.IStringLocalizer<atelier_platform_aplicaciones_web.IAM.Resources.IamMessages> localizer,
    atelier_platform_aplicaciones_web.Shared.Interfaces.Rest.ProblemDetails.ProblemDetailsFactory problemDetailsFactory) : ControllerBase
{
    [HttpPost("sign-in")]
    public async Task<IActionResult> SignIn([FromBody] SignInResource resource, CancellationToken cancellationToken)
    {
        var command = SignInCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await userCommandService.Handle(command, cancellationToken);
        
        if (!result.IsSuccess)
            return IamErrorToActionAssembler.ToActionResult(result.Error, result.Message, this, problemDetailsFactory, localizer);

        var authenticatedUserResource = AuthenticatedUserResourceFromEntityAssembler.ToResourceFromEntity(result.Value!.User, result.Value.Token);
        return Ok(authenticatedUserResource);
    }

    [HttpPost("google-sign-in")]
    public async Task<IActionResult> GoogleSignIn([FromBody] GoogleSignInResource resource, CancellationToken cancellationToken)
    {
        var command = GoogleSignInCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await userCommandService.Handle(command, cancellationToken);
        
        if (!result.IsSuccess)
            return IamErrorToActionAssembler.ToActionResult(result.Error, result.Message, this, problemDetailsFactory, localizer);

        var authenticatedUserResource = AuthenticatedUserResourceFromEntityAssembler.ToResourceFromEntity(result.Value!.User, result.Value.Token);
        return Ok(authenticatedUserResource);
    }

    [HttpPost("sign-up")]
    public async Task<IActionResult> SignUp([FromBody] SignUpResource resource, CancellationToken cancellationToken)
    {
        var command = SignUpCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await userCommandService.Handle(command, cancellationToken);

        if (!result.IsSuccess)
            return IamErrorToActionAssembler.ToActionResult(result.Error, result.Message, this, problemDetailsFactory, localizer);

        return Ok(new { Message = "User created successfully." });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] PasswordRecoveryResource resource, CancellationToken cancellationToken)
    {
        var command = GeneratePasswordRecoveryTokenCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await passwordRecoveryCommandService.Handle(command, cancellationToken);

        if (!result.IsSuccess)
            return IamErrorToActionAssembler.ToActionResult(result.Error, result.Message, this, problemDetailsFactory, localizer);

        return Ok(new { Message = "Password recovery token generated and sent." });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordResource resource, CancellationToken cancellationToken)
    {
        var command = ResetPasswordCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await passwordRecoveryCommandService.Handle(command, cancellationToken);

        if (!result.IsSuccess)
            return IamErrorToActionAssembler.ToActionResult(result.Error, result.Message, this, problemDetailsFactory, localizer);

        return Ok(new { Message = "Password has been successfully reset." });
    }
}
