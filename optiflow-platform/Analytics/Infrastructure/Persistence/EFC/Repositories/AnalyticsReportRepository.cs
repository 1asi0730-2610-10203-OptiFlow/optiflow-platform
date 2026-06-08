using optiflow_platform.Analytics.Domain.Model.Aggregates;
using optiflow_platform.Analytics.Domain.Repositories;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace optiflow_platform.Analytics.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     EF Core implementation of <see cref="IAnalyticsReportRepository"/>.
/// </summary>
/// <param name="context">The application database context.</param>
public class AnalyticsReportRepository(AppDbContext context)
    : BaseRepository<AnalyticsReport>(context), IAnalyticsReportRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<AnalyticsReport>> FindByPeriodAsync(
        string period,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<AnalyticsReport>()
            .Where(r => r.Period == period)
            .ToListAsync(cancellationToken);
    }
}
