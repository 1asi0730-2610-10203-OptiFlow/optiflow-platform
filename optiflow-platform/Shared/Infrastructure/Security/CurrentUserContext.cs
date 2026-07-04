using System.Security.Claims;
using optiflow_platform.Shared.Application.Services;
using Microsoft.AspNetCore.Http;

namespace optiflow_platform.Shared.Infrastructure.Security;

public class CurrentUserContext(IHttpContextAccessor httpContextAccessor) : ICurrentUserContext
{
    public Guid? UserId => ParseClaim(ClaimTypes.NameIdentifier);
    public Guid? AccountId => ParseClaim("AccountId");

    private Guid? ParseClaim(string claimType)
    {
        var value = httpContextAccessor.HttpContext?.User?.FindFirst(claimType)?.Value;
        return Guid.TryParse(value, out var parsed) ? parsed : null;
    }
}
