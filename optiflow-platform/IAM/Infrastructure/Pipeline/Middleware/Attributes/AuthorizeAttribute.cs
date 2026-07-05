using System;
using System.Linq;
using System.Threading.Tasks;
using optiflow_platform.IAM.Domain.Model.Aggregates;
using optiflow_platform.Subscription.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace optiflow_platform.IAM.Infrastructure.Pipeline.Middleware.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var metadata = context.ActionDescriptor.EndpointMetadata;

        // If the action has [AllowAnonymous], skip authorization
        if (metadata.OfType<AllowAnonymousAttribute>().Any())
            return;

        // Verify if the user is attached to the HttpContext
        var user = context.HttpContext.Items["User"] as User;
        if (user == null)
        {
            context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            return;
        }

        // Block access to account-scoped endpoints until the user has completed onboarding
        var allowWithoutAccount = metadata.OfType<AllowWithoutAccountAttribute>().Any();
        if (!allowWithoutAccount && user.AccountId == null)
        {
            context.Result = new JsonResult(new { message = "Account setup required", code = "ACCOUNT_SETUP_REQUIRED" })
                { StatusCode = StatusCodes.Status403Forbidden };
            return;
        }

        // Require an active subscription for the account before serving the dashboard. Purchase,
        // onboarding, profile and the client-facing portal opt out via [AllowWithoutSubscription].
        var allowWithoutSubscription = metadata.OfType<AllowWithoutSubscriptionAttribute>().Any();
        if (!allowWithoutSubscription && user.AccountId != null)
        {
            var subscriptions = context.HttpContext.RequestServices.GetRequiredService<ISubscriptionQueryService>();
            var active = await subscriptions.GetCurrentActiveSubscriptionAsync(context.HttpContext.RequestAborted);
            if (active == null)
            {
                context.Result = new JsonResult(new { message = "Active subscription required", code = "SUBSCRIPTION_REQUIRED" })
                    { StatusCode = StatusCodes.Status403Forbidden };
            }
        }
    }
}
