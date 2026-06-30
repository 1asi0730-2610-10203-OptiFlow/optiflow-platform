using System.Threading;
using System.Threading.Tasks;
using optiflow_platform.IAM.Domain.Model.Entities;
using optiflow_platform.IAM.Domain.Repositories;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace optiflow_platform.IAM.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class PasswordRecoveryTokenRepository(AppDbContext context) : BaseRepository<PasswordRecoveryToken>(context), IPasswordRecoveryTokenRepository
{
    public async Task<PasswordRecoveryToken?> FindByTokenHashAsync(string tokenHash, CancellationToken cancellationToken)
    {
        return await Context.Set<PasswordRecoveryToken>().FirstOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken);
    }
}
