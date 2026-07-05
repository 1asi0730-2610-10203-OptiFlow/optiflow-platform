using optiflow_platform.IAM.Domain.Model.Aggregates;
using optiflow_platform.IAM.Domain.Model.Commands;
using optiflow_platform.IAM.Domain.Model.ValueObjects;
using optiflow_platform.IAM.Interfaces.REST.Resources;

namespace optiflow_platform.IAM.Interfaces.REST.Transform;

public static class SignUpCommandFromResourceAssembler
{
    public static SignUpCommand ToCommandFromResource(SignUpResource resource)
    {
        var role = string.Equals(resource.UserType, "client", System.StringComparison.OrdinalIgnoreCase)
            ? UserRole.Client
            : UserRole.Admin;
        return new SignUpCommand(new EmailAddress(resource.Email), new Password(resource.Password), role);
    }
}
