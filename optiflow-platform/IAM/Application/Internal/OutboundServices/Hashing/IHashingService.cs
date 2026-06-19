namespace atelier_platform_aplicaciones_web.IAM.Application.Internal.OutboundServices.Hashing;

public interface IHashingService
{
    string Encode(string rawPassword);
    bool Matches(string rawPassword, string encodedPassword);
}
