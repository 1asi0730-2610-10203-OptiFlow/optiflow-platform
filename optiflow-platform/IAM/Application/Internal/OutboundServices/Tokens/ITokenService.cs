namespace optiflow_platform.IAM.Application.Internal.OutboundServices.Tokens;

public interface ITokenService
{
    string GenerateToken(string username);
    string? GetUsernameFromToken(string token);
    bool ValidateToken(string token);
}
