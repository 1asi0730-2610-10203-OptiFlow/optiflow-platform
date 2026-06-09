using Microsoft.EntityFrameworkCore;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Repositories;
using optiflow_platform.Subscription.Domain.Model.Aggregates;
using optiflow_platform.Subscription.Domain.Model.ValueObjects;
using optiflow_platform.Subscription.Domain.Repositories;

namespace optiflow_platform.Subscription.Infrastructure.Persistence.EFC.Repositories;

/// <summary>Entity Framework repository for billing persistence.</summary>
public class BillingRepository(AppDbContext context)
    : BaseRepository<Billing>(context), IBillingRepository
{
    /// <inheritdoc />
    public async Task<Billing?> FindBySubscriptionIdAsync(
        SubscriptionId subscriptionId, CancellationToken cancellationToken = default) =>
        await Context.Set<Billing>()
            .FirstOrDefaultAsync(b => b.SubscriptionId.Value == subscriptionId.Value, cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<Billing>> FindDueForRenewalAsync(
        DateTimeOffset date, CancellationToken cancellationToken = default) =>
        await Context.Set<Billing>()
            .Where(b => b.AutoRenew && b.RenewalDate <= date && b.BillingStatus == Billing.StatusDue)
            .ToListAsync(cancellationToken);
}
