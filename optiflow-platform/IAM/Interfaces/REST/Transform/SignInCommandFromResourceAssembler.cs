using optiflow_platform.IAM.Domain.Model.Commands;
using optiflow_platform.IAM.Domain.Model.ValueObjects;
using optiflow_platform.IAM.Interfaces.REST.Resources;

namespace optiflow_platform.IAM.Interfaces.REST.Transform;

public static class SignInCommandFromResourceAssembler
{
    public static SignInCommand ToCommandFromResource(SignInResource resource)
    {
        return new SignInCommand(new EmailAddress(resource.Email), new Password(resource.Password));
    }
}
