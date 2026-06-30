using System;

namespace optiflow_platform.IAM.Domain.Model.ValueObjects;

public record GoogleId
{
    public GoogleId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("iam.error.googleToken.invalid");
        }
        Value = value;
    }

    public string Value { get; init; }
}
