using atelier_platform_aplicaciones_web.IAM.Infrastructure.Pipeline.Middleware.Components;
using Microsoft.AspNetCore.Builder;

namespace atelier_platform_aplicaciones_web.IAM.Infrastructure.Pipeline.Middleware.Extensions;

public static class RequestAuthorizationMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestAuthorization(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<JwtMiddleware>();
    }
}
