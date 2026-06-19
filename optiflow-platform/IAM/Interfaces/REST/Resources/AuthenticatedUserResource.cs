using System;

namespace atelier_platform_aplicaciones_web.IAM.Interfaces.REST.Resources;

public record AuthenticatedUserResource(Guid Id, string Email, string Token);
