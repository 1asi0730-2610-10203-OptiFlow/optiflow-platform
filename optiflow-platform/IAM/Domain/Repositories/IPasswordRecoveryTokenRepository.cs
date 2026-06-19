using System.Threading;
using System.Threading.Tasks;
using atelier_platform_aplicaciones_web.IAM.Domain.Model.Entities;
using atelier_platform_aplicaciones_web.Shared.Domain.Repositories;

namespace atelier_platform_aplicaciones_web.IAM.Domain.Repositories;

public interface IPasswordRecoveryTokenRepository : IBaseRepository<PasswordRecoveryToken>
{
    Task<PasswordRecoveryToken?> FindByTokenHashAsync(string tokenHash, CancellationToken cancellationToken);
}
