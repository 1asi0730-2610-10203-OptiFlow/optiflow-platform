using optiflow_platform.IAM.Domain.Model.Commands;
using optiflow_platform.IAM.Domain.Model.ValueObjects;
using optiflow_platform.IAM.Interfaces.REST.Resources;

namespace optiflow_platform.IAM.Interfaces.REST.Transform;

public static class GeneratePasswordRecoveryTokenCommandFromResourceAssembler
{
    public static GeneratePasswordRecoveryTokenCommand ToCommandFromResource(PasswordRecoveryResource resource)
    {
        return new GeneratePasswordRecoveryTokenCommand(new EmailAddress(resource.Email));
    }
}
