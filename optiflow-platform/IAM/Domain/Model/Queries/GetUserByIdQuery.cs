using System;
using optiflow_platform.IAM.Domain.Model.ValueObjects;

namespace optiflow_platform.IAM.Domain.Model.Queries;

public record GetUserByIdQuery(UserId UserId);
