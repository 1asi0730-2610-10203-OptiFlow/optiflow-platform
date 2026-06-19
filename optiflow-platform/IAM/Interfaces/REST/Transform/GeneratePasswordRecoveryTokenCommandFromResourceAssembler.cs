using atelier_platform_aplicaciones_web.IAM.Domain.Model.Commands;
using atelier_platform_aplicaciones_web.IAM.Domain.Model.ValueObjects;
using atelier_platform_aplicaciones_web.IAM.Interfaces.REST.Resources;

namespace atelier_platform_aplicaciones_web.IAM.Interfaces.REST.Transform;

public static class GeneratePasswordRecoveryTokenCommandFromResourceAssembler
{
    public static GeneratePasswordRecoveryTokenCommand ToCommandFromResource(PasswordRecoveryResource resource)
    {
        return new GeneratePasswordRecoveryTokenCommand(new EmailAddress(resource.Email));
    }
}
