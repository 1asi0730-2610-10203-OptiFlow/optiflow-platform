namespace optiflow_platform.IAM.Application.Internal.OutboundServices.Hashing;

public interface IHashingService
{
    string Encode(string rawPassword);
    bool Matches(string rawPassword, string encodedPassword);
}
