using optiflow_platform.IAM.Domain.Model.Aggregates;
using optiflow_platform.IAM.Interfaces.REST.Resources;

namespace optiflow_platform.IAM.Interfaces.REST.Transform;

public static class AuthenticatedUserResourceFromEntityAssembler
{
    public static AuthenticatedUserResource ToResourceFromEntity(User user, string token)
    {
        return new AuthenticatedUserResource(user.Id.Value, user.Email.Value, token, user.AccountId, user.Role.ToString().ToUpper());
    }
}
