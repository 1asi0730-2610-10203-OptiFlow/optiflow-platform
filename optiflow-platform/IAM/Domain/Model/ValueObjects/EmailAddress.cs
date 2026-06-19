using System;

namespace atelier_platform_aplicaciones_web.IAM.Domain.Model.ValueObjects;

public record EmailAddress
{
    public EmailAddress(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("iam.error.email.required");
        }
        Value = value;
    }

    public string Value { get; init; }
}
