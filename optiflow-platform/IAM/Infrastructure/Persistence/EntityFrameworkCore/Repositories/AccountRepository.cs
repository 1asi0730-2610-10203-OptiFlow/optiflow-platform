using System.Threading;
using System.Threading.Tasks;
using optiflow_platform.IAM.Domain.Model.Aggregates;
using optiflow_platform.IAM.Domain.Repositories;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace optiflow_platform.IAM.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class AccountRepository(AppDbContext context) : BaseRepository<Account>(context), IAccountRepository
{
    public async Task<Account?> FindByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await Context.Set<Account>().FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }
}
