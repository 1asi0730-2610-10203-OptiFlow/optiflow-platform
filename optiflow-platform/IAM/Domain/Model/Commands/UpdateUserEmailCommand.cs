using System;
using optiflow_platform.IAM.Domain.Model.ValueObjects;

namespace optiflow_platform.IAM.Domain.Model.Commands;

public record UpdateUserEmailCommand(UserId UserId, EmailAddress NewEmail);
