using atelier_platform_aplicaciones_web.IAM.Domain.Model.Commands;
using atelier_platform_aplicaciones_web.IAM.Interfaces.REST.Resources;

namespace atelier_platform_aplicaciones_web.IAM.Interfaces.REST.Transform;

public static class GoogleSignInCommandFromResourceAssembler
{
    public static GoogleSignInCommand ToCommandFromResource(GoogleSignInResource resource)
    {
        return new GoogleSignInCommand(resource.IdToken);
    }
}
