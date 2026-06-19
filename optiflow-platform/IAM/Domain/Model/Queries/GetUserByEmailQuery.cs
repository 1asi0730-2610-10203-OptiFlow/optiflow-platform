using atelier_platform_aplicaciones_web.IAM.Domain.Model.ValueObjects;

namespace atelier_platform_aplicaciones_web.IAM.Domain.Model.Queries;

public record GetUserByEmailQuery(EmailAddress Email);
