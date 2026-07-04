using System;
using System.Linq;
using optiflow_platform.IAM.Domain.Model.Aggregates;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace optiflow_platform.IAM.Infrastructure.Pipeline.Middleware.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // If the action has [AllowAnonymous], skip authorization
        var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
        if (allowAnonymous)
            return;

        // Verify if the user is attached to the HttpContext
        var user = context.HttpContext.Items["User"] as User;
        if (user == null)
        {
            context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            return;
        }

        // Block access to account-scoped endpoints until the user has completed onboarding
        var allowWithoutAccount = context.ActionDescriptor.EndpointMetadata.OfType<AllowWithoutAccountAttribute>().Any();
        if (!allowWithoutAccount && user.AccountId == null)
        {
            context.Result = new JsonResult(new { message = "Account setup required", code = "ACCOUNT_SETUP_REQUIRED" })
                { StatusCode = StatusCodes.Status403Forbidden };
        }
    }
}
