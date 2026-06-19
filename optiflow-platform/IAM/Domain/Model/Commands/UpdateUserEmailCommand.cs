using System;
using atelier_platform_aplicaciones_web.IAM.Domain.Model.ValueObjects;

namespace atelier_platform_aplicaciones_web.IAM.Domain.Model.Commands;

public record UpdateUserEmailCommand(UserId UserId, EmailAddress NewEmail);
