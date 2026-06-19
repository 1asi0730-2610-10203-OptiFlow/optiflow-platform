using optiflow_platform.IAM.Domain.Model.Commands;
using optiflow_platform.IAM.Interfaces.REST.Resources;

namespace optiflow_platform.IAM.Interfaces.REST.Transform;

public static class GoogleSignInCommandFromResourceAssembler
{
    public static GoogleSignInCommand ToCommandFromResource(GoogleSignInResource resource)
    {
        return new GoogleSignInCommand(resource.IdToken);
    }
}
