using optiflow_platform.IAM.Domain.Model.Aggregates;
using optiflow_platform.IAM.Interfaces.REST.Resources;

namespace optiflow_platform.IAM.Interfaces.REST.Transform;

public static class AccountResourceFromEntityAssembler
{
    public static AccountResource ToResourceFromEntity(Account account)
    {
        return new AccountResource(account.Id, account.Name);
    }
}
