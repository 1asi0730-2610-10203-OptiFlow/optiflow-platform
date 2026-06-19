using atelier_platform_aplicaciones_web.IAM.Domain.Model.Commands;
using atelier_platform_aplicaciones_web.IAM.Domain.Model.ValueObjects;
using atelier_platform_aplicaciones_web.IAM.Interfaces.REST.Resources;

namespace atelier_platform_aplicaciones_web.IAM.Interfaces.REST.Transform;

public static class SignUpCommandFromResourceAssembler
{
    public static SignUpCommand ToCommandFromResource(SignUpResource resource)
    {
        return new SignUpCommand(new EmailAddress(resource.Email), new Password(resource.Password));
    }
}
