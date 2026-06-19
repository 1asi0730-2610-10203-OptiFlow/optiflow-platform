using optiflow_platform.IAM.Application.Internal.OutboundServices.Hashing;
using BCrypt.Net;

namespace optiflow_platform.IAM.Infrastructure.Hashing.BCrypt;

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
