using optiflow_platform.IAM.Domain.Model.ValueObjects;

namespace optiflow_platform.IAM.Domain.Model.Queries;

public record GetUserByEmailQuery(EmailAddress Email);
