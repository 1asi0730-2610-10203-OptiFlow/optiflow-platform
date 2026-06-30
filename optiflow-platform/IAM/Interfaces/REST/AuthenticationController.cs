using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using optiflow_platform.IAM.Application.CommandServices;
using optiflow_platform.IAM.Interfaces.REST.Resources;
using optiflow_platform.IAM.Interfaces.REST.Transform;
using optiflow_platform.IAM.Infrastructure.Pipeline.Middleware.Attributes;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.IAM.Interfaces.REST;

[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Authentication")]
[AllowAnonymous]
public class AuthenticationController(
    IUserCommandService userCommandService,
    IPasswordRecoveryCommandService passwordRecoveryCommandService,
    Microsoft.Extensions.Localization.IStringLocalizer<optiflow_platform.IAM.Resources.IamMessages> localizer,
    optiflow_platform.Shared.Interfaces.Rest.ProblemDetails.ProblemDetailsFactory problemDetailsFactory) : ControllerBase
{
    /// <summary>Inicia sesión con un usuario existente y genera un token JWT.</summary>
    [HttpPost("sign-in")]
    [SwaggerOperation(Summary = "Sign in", OperationId = "SignIn")]
    public async Task<IActionResult> SignIn([FromBody] SignInResource resource, CancellationToken cancellationToken)
    {
        var command = SignInCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await userCommandService.Handle(command, cancellationToken);
        
        if (!result.IsSuccess)
            return IamErrorToActionAssembler.ToActionResult(result.Error, result.Message, this, problemDetailsFactory, localizer);

        var authenticatedUserResource = AuthenticatedUserResourceFromEntityAssembler.ToResourceFromEntity(result.Value!.User, result.Value.Token);
        return Ok(authenticatedUserResource);
    }

    /// <summary>Inicia sesión utilizando una cuenta de Google.</summary>
    [HttpPost("sign-in/google")]
    [SwaggerOperation(Summary = "Google sign in", OperationId = "GoogleSignIn")]
    public async Task<IActionResult> GoogleSignIn([FromBody] GoogleSignInResource resource, CancellationToken cancellationToken)
    {
        var command = GoogleSignInCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await userCommandService.Handle(command, cancellationToken);
        
        if (!result.IsSuccess)
            return IamErrorToActionAssembler.ToActionResult(result.Error, result.Message, this, problemDetailsFactory, localizer);

        var authenticatedUserResource = AuthenticatedUserResourceFromEntityAssembler.ToResourceFromEntity(result.Value!.User, result.Value.Token);
        return Ok(authenticatedUserResource);
    }

    /// <summary>Registra un nuevo usuario en la plataforma.</summary>
    [HttpPost("sign-up")]
    [SwaggerOperation(Summary = "Sign up", OperationId = "SignUp")]
    public async Task<IActionResult> SignUp([FromBody] SignUpResource resource, CancellationToken cancellationToken)
    {
        var command = SignUpCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await userCommandService.Handle(command, cancellationToken);

        if (!result.IsSuccess)
            return IamErrorToActionAssembler.ToActionResult(result.Error, result.Message, this, problemDetailsFactory, localizer);

        var authenticatedUserResource = AuthenticatedUserResourceFromEntityAssembler.ToResourceFromEntity(result.Value!.User, result.Value.Token);
        return Ok(authenticatedUserResource);
    }

    /// <summary>Solicita la recuperación de contraseña enviando un token.</summary>
    [HttpPost("password-recoveries")]
    [SwaggerOperation(Summary = "Forgot password", OperationId = "ForgotPassword")]
    public async Task<IActionResult> ForgotPassword([FromBody] PasswordRecoveryResource resource, CancellationToken cancellationToken)
    {
        var command = GeneratePasswordRecoveryTokenCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await passwordRecoveryCommandService.Handle(command, cancellationToken);

        if (!result.IsSuccess)
            return IamErrorToActionAssembler.ToActionResult(result.Error, result.Message, this, problemDetailsFactory, localizer);

        return Ok(new { Message = "Password recovery token generated and sent." });
    }

    /// <summary>Restablece la contraseña utilizando un token de recuperación válido.</summary>
    [HttpPost("password-resets")]
    [SwaggerOperation(Summary = "Reset password", OperationId = "ResetPassword")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordResource resource, CancellationToken cancellationToken)
    {
        var command = ResetPasswordCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await passwordRecoveryCommandService.Handle(command, cancellationToken);

        if (!result.IsSuccess)
            return IamErrorToActionAssembler.ToActionResult(result.Error, result.Message, this, problemDetailsFactory, localizer);

        return Ok(new { Message = "Password has been successfully reset." });
    }
}
