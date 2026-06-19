using System;
using atelier_platform_aplicaciones_web.IAM.Domain.Model.Commands;
using atelier_platform_aplicaciones_web.IAM.Domain.Model.ValueObjects;
using atelier_platform_aplicaciones_web.IAM.Interfaces.REST.Resources;

namespace atelier_platform_aplicaciones_web.IAM.Interfaces.REST.Transform;

public static class UpdateUserEmailCommandFromResourceAssembler
{
    public static UpdateUserEmailCommand ToCommandFromResource(Guid userId, UpdateUserEmailResource resource)
    {
        return new UpdateUserEmailCommand(new UserId(userId), new EmailAddress(resource.NewEmail));
    }
}
