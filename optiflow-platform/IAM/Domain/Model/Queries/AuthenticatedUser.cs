using optiflow_platform.IAM.Domain.Model.Aggregates;

namespace optiflow_platform.IAM.Domain.Model.Queries;

public record AuthenticatedUser(User User, string Token);
