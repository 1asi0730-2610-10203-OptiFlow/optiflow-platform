using System;
using optiflow_platform.IAM.Domain.Model.Commands;
using optiflow_platform.IAM.Domain.Model.ValueObjects;
using optiflow_platform.IAM.Interfaces.REST.Resources;

namespace optiflow_platform.IAM.Interfaces.REST.Transform;

public static class UpdateUserEmailCommandFromResourceAssembler
{
    public static UpdateUserEmailCommand ToCommandFromResource(Guid userId, UpdateUserEmailResource resource)
    {
        return new UpdateUserEmailCommand(new UserId(userId), new EmailAddress(resource.NewEmail));
    }
}
