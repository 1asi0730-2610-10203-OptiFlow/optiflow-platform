using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using atelier_platform_aplicaciones_web.IAM.Application.Internal.OutboundServices.Tokens;
using atelier_platform_aplicaciones_web.IAM.Infrastructure.Tokens.Jwt.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace atelier_platform_aplicaciones_web.IAM.Infrastructure.Tokens.Jwt.Services;

public class TokenService(IOptions<TokenSettings> tokenSettings) : ITokenService
{
    private readonly TokenSettings _tokenSettings = tokenSettings.Value;

    public string GenerateToken(string username)
    {
        var secret = _tokenSettings.Secret;
        if (string.IsNullOrEmpty(secret)) throw new Exception("JWT Secret not configured");
        var key = Encoding.ASCII.GetBytes(secret);

        var expirationDays = _tokenSettings.ExpirationDays > 0 ? _tokenSettings.ExpirationDays : 7;

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, username) }),
            Expires = DateTime.UtcNow.AddDays(expirationDays),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public string? GetUsernameFromToken(string token)
    {
        if (string.IsNullOrEmpty(token)) return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var secret = _tokenSettings.Secret;
        if (string.IsNullOrEmpty(secret)) throw new Exception("JWT Secret not configured");
        var key = Encoding.ASCII.GetBytes(secret);

        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var usernameClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            return usernameClaim?.Value;
        }
        catch (Exception)
        {
            return null; // Return null if validation fails
        }
    }

    public bool ValidateToken(string token)
    {
        return GetUsernameFromToken(token) != null;
    }
}
