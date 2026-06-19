namespace optiflow_platform.IAM.Interfaces.REST.Resources;

public record UpdateUserPasswordResource(string CurrentPassword, string NewPassword);
