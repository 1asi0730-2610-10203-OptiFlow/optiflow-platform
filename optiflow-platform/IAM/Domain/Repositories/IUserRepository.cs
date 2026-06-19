using System.Threading;
using System.Threading.Tasks;
using atelier_platform_aplicaciones_web.IAM.Domain.Model.Aggregates;
using atelier_platform_aplicaciones_web.IAM.Domain.Model.ValueObjects;
using atelier_platform_aplicaciones_web.Shared.Domain.Repositories;

namespace atelier_platform_aplicaciones_web.IAM.Domain.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> FindByEmailAsync(EmailAddress email, CancellationToken cancellationToken);
    Task<bool> ExistsByEmailAsync(EmailAddress email, CancellationToken cancellationToken);
    Task<User?> FindUserByIdAsync(UserId id, CancellationToken cancellationToken);
}
