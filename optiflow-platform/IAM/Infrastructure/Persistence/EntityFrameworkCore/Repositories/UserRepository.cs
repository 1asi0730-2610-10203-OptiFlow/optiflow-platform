using System.Threading;
using System.Threading.Tasks;
using optiflow_platform.IAM.Domain.Model.Aggregates;
using optiflow_platform.IAM.Domain.Repositories;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

using optiflow_platform.IAM.Domain.Model.ValueObjects;

namespace optiflow_platform.IAM.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class UserRepository(AppDbContext context) : BaseRepository<User>(context), IUserRepository
{
    public async Task<User?> FindByEmailAsync(EmailAddress email, CancellationToken cancellationToken)
    {
        return await Context.Set<User>().FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(EmailAddress email, CancellationToken cancellationToken)
    {
        return await Context.Set<User>().AnyAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<User?> FindByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var userId = new UserId(id);
        return await Context.Set<User>().FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    public async Task<User?> FindUserByIdAsync(UserId id, CancellationToken cancellationToken)
    {
        return await Context.Set<User>().FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }
}
