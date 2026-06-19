using System;

namespace optiflow_platform.IAM.Domain.Model.ValueObjects;

public record UserId
{
    public UserId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("iam.error.userId.required");
        }
        Value = value;
    }

    public Guid Value { get; init; }
}
