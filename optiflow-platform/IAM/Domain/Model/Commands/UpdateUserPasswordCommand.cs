using System;
using optiflow_platform.IAM.Domain.Model.ValueObjects;

namespace optiflow_platform.IAM.Domain.Model.Commands;

public record UpdateUserPasswordCommand(UserId UserId, Password CurrentPassword, Password NewPassword);
