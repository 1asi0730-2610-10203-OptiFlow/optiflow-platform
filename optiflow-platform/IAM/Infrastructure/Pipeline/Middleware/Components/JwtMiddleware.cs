using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using optiflow_platform.IAM.Application.Internal.OutboundServices.Tokens;
using optiflow_platform.IAM.Application.QueryServices;
using optiflow_platform.IAM.Domain.Model.Queries;
using optiflow_platform.IAM.Domain.Model.ValueObjects;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.AspNetCore.Http;

namespace optiflow_platform.IAM.Infrastructure.Pipeline.Middleware.Components;

public class JwtMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context, ITokenService tokenService, IUserQueryService userQueryService, AppDbContext appDbContext)
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
                appDbContext.CurrentAccountId = user.AccountId;

                // Create a ClaimsPrincipal so the AuditableEntityInterceptor
                // (and any other infrastructure) can resolve the current user ID.
                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, user.Id.Value.ToString()),
                    new(ClaimTypes.Email, user.Email.Value),
                };
                if (user.AccountId is not null)
                    claims.Add(new Claim("AccountId", user.AccountId.Value.ToString()));

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

