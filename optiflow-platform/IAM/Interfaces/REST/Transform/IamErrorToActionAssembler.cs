using System;
using atelier_platform_aplicaciones_web.IAM.Domain.Model;
using atelier_platform_aplicaciones_web.IAM.Domain.Model.Aggregates;
using atelier_platform_aplicaciones_web.Shared.Application.Model;
using atelier_platform_aplicaciones_web.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using atelier_platform_aplicaciones_web.IAM.Resources;

namespace atelier_platform_aplicaciones_web.IAM.Interfaces.REST.Transform;

public static class IamErrorToActionAssembler
{
    public static IActionResult ToActionResult(Enum? error, string message, ControllerBase controller, ProblemDetailsFactory problemDetailsFactory, IStringLocalizer<IamMessages> localizer)
    {
        if (error is IamError iamError)
        {
            var translatedMessage = localizer[message].Value ?? message;
            return iamError switch
            {
                IamError.UserNotFound => problemDetailsFactory.CreateProblemDetails(controller, StatusCodes.Status404NotFound, iamError, translatedMessage, "User Not Found"),
                IamError.EmailAlreadyInUse => new ConflictObjectResult(new { error = translatedMessage }),
                IamError.InvalidCredentials => new UnauthorizedObjectResult(new { error = translatedMessage }),
                IamError.InvalidGoogleToken => new UnauthorizedObjectResult(new { error = translatedMessage }),
                IamError.InvalidCurrentPassword => new UnauthorizedObjectResult(new { error = translatedMessage }),
                IamError.InvalidOrExpiredToken => new UnauthorizedObjectResult(new { error = translatedMessage }),
                IamError.ExpiredOrUsedToken => new UnauthorizedObjectResult(new { error = translatedMessage }),
                IamError.DatabaseError => problemDetailsFactory.CreateProblemDetails(controller, StatusCodes.Status500InternalServerError, iamError, translatedMessage, "Internal Server Error"),
                IamError.InternalServerError => problemDetailsFactory.CreateProblemDetails(controller, StatusCodes.Status500InternalServerError, iamError, translatedMessage, "Internal Server Error"),
                _ => problemDetailsFactory.CreateProblemDetails(controller, StatusCodes.Status500InternalServerError, null, "Unexpected Error", "Internal Server Error")
            };
        }

        return problemDetailsFactory.CreateProblemDetails(controller, StatusCodes.Status500InternalServerError, null, "Unexpected Error", "Internal Server Error");
    }
}
