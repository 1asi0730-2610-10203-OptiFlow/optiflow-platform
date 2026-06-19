using System;

namespace optiflow_platform.IAM.Interfaces.REST.Resources;

public record AuthenticatedUserResource(Guid Id, string Email, string Token);
