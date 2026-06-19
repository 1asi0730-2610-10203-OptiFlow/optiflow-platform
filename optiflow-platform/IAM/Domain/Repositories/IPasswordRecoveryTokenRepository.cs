using System.Threading;
using System.Threading.Tasks;
using optiflow_platform.IAM.Domain.Model.Entities;
using optiflow_platform.Shared.Domain.Repositories;

namespace optiflow_platform.IAM.Domain.Repositories;

public interface IPasswordRecoveryTokenRepository : IBaseRepository<PasswordRecoveryToken>
{
    Task<PasswordRecoveryToken?> FindByTokenHashAsync(string tokenHash, CancellationToken cancellationToken);
}
