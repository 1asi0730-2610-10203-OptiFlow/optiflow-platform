using System.Threading;
using System.Threading.Tasks;
using optiflow_platform.IAM.Application.QueryServices;
using optiflow_platform.IAM.Domain.Model.Aggregates;
using optiflow_platform.IAM.Domain.Model.Queries;
using optiflow_platform.IAM.Domain.Repositories;

namespace optiflow_platform.IAM.Application.Internal.QueryServices;

public class AccountQueryService(IAccountRepository accountRepository) : IAccountQueryService
{
    public async Task<Account?> Handle(GetAccountByIdQuery query, CancellationToken cancellationToken)
    {
        return await accountRepository.FindByIdAsync(query.AccountId, cancellationToken);
    }
}
