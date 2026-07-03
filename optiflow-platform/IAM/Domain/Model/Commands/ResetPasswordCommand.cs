using optiflow_platform.IAM.Domain.Model.ValueObjects;

namespace optiflow_platform.IAM.Domain.Model.Commands;

public record ResetPasswordCommand(string Token, Password NewPassword);
