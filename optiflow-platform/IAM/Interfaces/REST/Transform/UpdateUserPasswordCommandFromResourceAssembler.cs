using System;
using optiflow_platform.IAM.Domain.Model.Commands;
using optiflow_platform.IAM.Domain.Model.ValueObjects;
using optiflow_platform.IAM.Interfaces.REST.Resources;

namespace optiflow_platform.IAM.Interfaces.REST.Transform;

public static class UpdateUserPasswordCommandFromResourceAssembler
{
    public static UpdateUserPasswordCommand ToCommandFromResource(Guid userId, UpdateUserPasswordResource resource)
    {
        return new UpdateUserPasswordCommand(new UserId(userId), new Password(resource.CurrentPassword), new Password(resource.NewPassword));
    }
}
