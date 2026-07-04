using System;
using optiflow_platform.IAM.Domain.Model.Commands;
using optiflow_platform.IAM.Interfaces.REST.Resources;

namespace optiflow_platform.IAM.Interfaces.REST.Transform;

public static class CreateAccountCommandFromResourceAssembler
{
    public static CreateAccountCommand ToCommandFromResource(CreateAccountResource resource, Guid ownerUserId)
    {
        return new CreateAccountCommand(resource.Name, ownerUserId);
    }
}
