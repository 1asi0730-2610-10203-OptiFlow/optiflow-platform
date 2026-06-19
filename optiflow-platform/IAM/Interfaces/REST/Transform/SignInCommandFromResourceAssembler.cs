using atelier_platform_aplicaciones_web.IAM.Domain.Model.Commands;
using atelier_platform_aplicaciones_web.IAM.Domain.Model.ValueObjects;
using atelier_platform_aplicaciones_web.IAM.Interfaces.REST.Resources;

namespace atelier_platform_aplicaciones_web.IAM.Interfaces.REST.Transform;

public static class SignInCommandFromResourceAssembler
{
    public static SignInCommand ToCommandFromResource(SignInResource resource)
    {
        return new SignInCommand(new EmailAddress(resource.Email), new Password(resource.Password));
    }
}
