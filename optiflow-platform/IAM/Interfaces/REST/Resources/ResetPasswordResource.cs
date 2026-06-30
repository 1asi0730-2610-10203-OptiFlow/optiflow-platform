namespace optiflow_platform.IAM.Interfaces.REST.Resources;

public record ResetPasswordResource(string Token, string NewPassword);
