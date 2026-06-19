namespace atelier_platform_aplicaciones_web.IAM.Interfaces.REST.Resources;

public record UpdateUserPasswordResource(string CurrentPassword, string NewPassword);
