using System.Threading;
using System.Threading.Tasks;
using atelier_platform_aplicaciones_web.IAM.Domain.Model.Entities;
using atelier_platform_aplicaciones_web.IAM.Domain.Repositories;
using atelier_platform_aplicaciones_web.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using atelier_platform_aplicaciones_web.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace atelier_platform_aplicaciones_web.IAM.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class PasswordRecoveryTokenRepository(AppDbContext context) : BaseRepository<PasswordRecoveryToken>(context), IPasswordRecoveryTokenRepository
{
    public async Task<PasswordRecoveryToken?> FindByTokenHashAsync(string tokenHash, CancellationToken cancellationToken)
    {
        return await Context.Set<PasswordRecoveryToken>().FirstOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken);
    }
}
