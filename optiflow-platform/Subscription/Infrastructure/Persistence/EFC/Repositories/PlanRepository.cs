using Microsoft.EntityFrameworkCore;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Repositories;
using optiflow_platform.Subscription.Domain.Model.Aggregates;
using optiflow_platform.Subscription.Domain.Repositories;

namespace optiflow_platform.Subscription.Infrastructure.Persistence.EFC.Repositories;

/// <summary>Entity Framework repository for plan persistence.</summary>
public class PlanRepository(AppDbContext context)
    : BaseRepository<Plan>(context), IPlanRepository
{
    /// <inheritdoc />
    public async Task<Plan?> FindByNameAsync(string name, CancellationToken cancellationToken = default) =>
        await Context.Set<Plan>()
            .FirstOrDefaultAsync(p => p.Name == name, cancellationToken);
}
