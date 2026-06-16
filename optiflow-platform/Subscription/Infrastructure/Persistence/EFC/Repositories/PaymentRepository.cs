using Microsoft.EntityFrameworkCore;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Repositories;
using optiflow_platform.Subscription.Domain.Model.Aggregates;
using optiflow_platform.Subscription.Domain.Model.ValueObjects;
using optiflow_platform.Subscription.Domain.Repositories;

namespace optiflow_platform.Subscription.Infrastructure.Persistence.EFC.Repositories;

/// <summary>Entity Framework repository for payment persistence.</summary>
public class PaymentRepository(AppDbContext context)
    : BaseRepository<Payment>(context), IPaymentRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<Payment>> FindBySubscriptionIdAsync(
        SubscriptionId subscriptionId, CancellationToken cancellationToken = default) =>
        await Context.Set<Payment>()
            .Where(p => p.SubscriptionId.Value == subscriptionId.Value)
            .ToListAsync(cancellationToken);
}
