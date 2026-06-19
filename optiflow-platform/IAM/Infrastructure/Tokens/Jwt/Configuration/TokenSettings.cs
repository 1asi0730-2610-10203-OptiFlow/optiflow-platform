namespace optiflow_platform.IAM.Infrastructure.Tokens.Jwt.Configuration;

public class TokenSettings
{
    public required string Secret { get; set; }
    public int ExpirationDays { get; set; }
}
