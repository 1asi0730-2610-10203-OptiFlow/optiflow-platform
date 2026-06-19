using atelier_platform_aplicaciones_web.IAM.Application.Internal.OutboundServices.Hashing;
using BCrypt.Net;

namespace atelier_platform_aplicaciones_web.IAM.Infrastructure.Hashing.BCrypt;

public class BCryptHashingService : IHashingService
{
    public string Encode(string rawPassword)
    {
        return global::BCrypt.Net.BCrypt.HashPassword(rawPassword);
    }

    public bool Matches(string rawPassword, string encodedPassword)
    {
        return global::BCrypt.Net.BCrypt.Verify(rawPassword, encodedPassword);
    }
}
