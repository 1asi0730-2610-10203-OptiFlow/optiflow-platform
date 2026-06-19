using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using atelier_platform_aplicaciones_web.IAM.Application.Internal.OutboundServices.Tokens;
using atelier_platform_aplicaciones_web.IAM.Application.QueryServices;
using atelier_platform_aplicaciones_web.IAM.Domain.Model.Queries;
using atelier_platform_aplicaciones_web.IAM.Domain.Model.ValueObjects;
using Microsoft.AspNetCore.Http;

namespace atelier_platform_aplicaciones_web.IAM.Infrastructure.Pipeline.Middleware.Components;

public class JwtMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context, ITokenService tokenService, IUserQueryService userQueryService)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        System.Console.WriteLine($"Token received: {token}");

        var userEmailString = tokenService.GetUsernameFromToken(token ?? "");
        System.Console.WriteLine($"Extracted email: {userEmailString}");
        
        if (!string.IsNullOrEmpty(userEmailString))
        {
            var user = await userQueryService.Handle(new GetUserByEmailQuery(new EmailAddress(userEmailString)), default);
            if (user != null)
            {
                context.Items["User"] = user;

                // Create a ClaimsPrincipal so the AuditableEntityInterceptor
                // (and any other infrastructure) can resolve the current user ID.
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.Value.ToString()),
                    new Claim(ClaimTypes.Email, user.Email.Value),
                };
                var identity = new ClaimsIdentity(claims, "Bearer");
                context.User = new ClaimsPrincipal(identity);

                System.Console.WriteLine($"User {userEmailString} attached to context");
            }
            else
            {
                System.Console.WriteLine($"User {userEmailString} NOT found in database");
            }
        }

        await next(context);
    }
}

