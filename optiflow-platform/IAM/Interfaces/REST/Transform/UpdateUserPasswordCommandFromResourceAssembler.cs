using System;
using atelier_platform_aplicaciones_web.IAM.Domain.Model.Commands;
using atelier_platform_aplicaciones_web.IAM.Domain.Model.ValueObjects;
using atelier_platform_aplicaciones_web.IAM.Interfaces.REST.Resources;

namespace atelier_platform_aplicaciones_web.IAM.Interfaces.REST.Transform;

public static class UpdateUserPasswordCommandFromResourceAssembler
{
    public static UpdateUserPasswordCommand ToCommandFromResource(Guid userId, UpdateUserPasswordResource resource)
    {
        return new UpdateUserPasswordCommand(new UserId(userId), new Password(resource.CurrentPassword), new Password(resource.NewPassword));
    }
}
