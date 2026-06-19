using optiflow_platform.IAM.Infrastructure.Pipeline.Middleware.Components;
using Microsoft.AspNetCore.Builder;

namespace optiflow_platform.IAM.Infrastructure.Pipeline.Middleware.Extensions;

public static class RequestAuthorizationMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestAuthorization(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<JwtMiddleware>();
    }
}
