using System;

namespace atelier_platform_aplicaciones_web.IAM.Domain.Model.ValueObjects;

public record Password
{
    public Password(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("iam.error.currentPassword.required");
        }
        Value = value;
    }

    public string Value { get; init; }
}
