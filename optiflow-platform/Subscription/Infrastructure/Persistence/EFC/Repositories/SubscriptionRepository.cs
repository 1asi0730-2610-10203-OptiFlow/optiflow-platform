using Microsoft.EntityFrameworkCore;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Repositories;
using optiflow_platform.Subscription.Domain.Repositories;

namespace optiflow_platform.Subscription.Infrastructure.Persistence.EFC.Repositories;

/// <summary>Entity Framework repository for subscription persistence.</summary>
public class SubscriptionRepository(AppDbContext context)
    : BaseRepository<Domain.Model.Aggregates.Subscription>(context), ISubscriptionRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<Domain.Model.Aggregates.Subscription>> FindByAdminIdAsync(
        int adminId, CancellationToken cancellationToken = default) =>
        await Context.Set<Domain.Model.Aggregates.Subscription>()
            .Where(s => s.AdminId == adminId)
            .ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<Domain.Model.Aggregates.Subscription>> FindByStatusAsync(
        string status, CancellationToken cancellationToken = default)
    {
        var target = new Domain.Model.ValueObjects.SubscriptionStatus(status);
        return await Context.Set<Domain.Model.Aggregates.Subscription>()
            .Where(s => s.Status == target)
            .ToListAsync(cancellationToken);
    }
}
