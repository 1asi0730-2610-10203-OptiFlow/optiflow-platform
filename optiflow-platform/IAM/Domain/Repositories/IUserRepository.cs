using System.Threading;
using System.Threading.Tasks;
using optiflow_platform.IAM.Domain.Model.Aggregates;
using optiflow_platform.IAM.Domain.Model.ValueObjects;
using optiflow_platform.Shared.Domain.Repositories;

namespace optiflow_platform.IAM.Domain.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> FindByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<User?> FindByEmailAsync(EmailAddress email, CancellationToken cancellationToken);
    Task<bool> ExistsByEmailAsync(EmailAddress email, CancellationToken cancellationToken);
    Task<User?> FindUserByIdAsync(UserId id, CancellationToken cancellationToken);
}
