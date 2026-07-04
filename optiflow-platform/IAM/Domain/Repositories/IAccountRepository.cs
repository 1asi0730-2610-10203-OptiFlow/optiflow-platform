using System.Threading;
using System.Threading.Tasks;
using optiflow_platform.IAM.Domain.Model.Aggregates;
using optiflow_platform.Shared.Domain.Repositories;

namespace optiflow_platform.IAM.Domain.Repositories;

public interface IAccountRepository : IBaseRepository<Account>
{
    Task<Account?> FindByIdAsync(Guid id, CancellationToken cancellationToken);
}
