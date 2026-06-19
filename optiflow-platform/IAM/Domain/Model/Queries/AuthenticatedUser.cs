using atelier_platform_aplicaciones_web.IAM.Domain.Model.Aggregates;

namespace atelier_platform_aplicaciones_web.IAM.Domain.Model.Queries;

public record AuthenticatedUser(User User, string Token);
