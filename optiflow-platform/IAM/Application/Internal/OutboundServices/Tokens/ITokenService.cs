namespace atelier_platform_aplicaciones_web.IAM.Application.Internal.OutboundServices.Tokens;

public interface ITokenService
{
    string GenerateToken(string username);
    string? GetUsernameFromToken(string token);
    bool ValidateToken(string token);
}
