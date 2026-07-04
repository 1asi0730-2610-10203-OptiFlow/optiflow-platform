using System;
using optiflow_platform.IAM.Domain.Model;
using optiflow_platform.IAM.Domain.Model.Aggregates;
using optiflow_platform.Shared.Application.Model;
using optiflow_platform.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using optiflow_platform.IAM.Resources;

namespace optiflow_platform.IAM.Interfaces.REST.Transform;

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
                IamError.AccountAlreadyOnboarded => new ConflictObjectResult(new { error = translatedMessage }),
                _ => problemDetailsFactory.CreateProblemDetails(controller, StatusCodes.Status500InternalServerError, null, "Unexpected Error", "Internal Server Error")
            };
        }

        return problemDetailsFactory.CreateProblemDetails(controller, StatusCodes.Status500InternalServerError, null, "Unexpected Error", "Internal Server Error");
    }
}
