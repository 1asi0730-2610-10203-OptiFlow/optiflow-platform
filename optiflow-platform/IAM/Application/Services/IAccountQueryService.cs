using System.Threading;
using System.Threading.Tasks;
using optiflow_platform.IAM.Domain.Model.Aggregates;
using optiflow_platform.IAM.Domain.Model.Queries;

namespace optiflow_platform.IAM.Application.QueryServices;

public interface IAccountQueryService
{
    Task<Account?> Handle(GetAccountByIdQuery query, CancellationToken cancellationToken);
}
