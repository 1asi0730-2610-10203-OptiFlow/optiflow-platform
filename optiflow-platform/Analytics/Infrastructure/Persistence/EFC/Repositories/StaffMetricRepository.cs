using optiflow_platform.Analytics.Domain.Model.Entities;
using optiflow_platform.Analytics.Domain.Repositories;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace optiflow_platform.Analytics.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     EF Core implementation of <see cref="IStaffMetricRepository"/>.
/// </summary>
/// <param name="context">The application database context.</param>
public class StaffMetricRepository(AppDbContext context)
    : BaseRepository<StaffMetric>(context), IStaffMetricRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<StaffMetric>> FindByReportIdAsync(
        int reportId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<StaffMetric>()
            .Where(m => m.ReportId == reportId)
            .ToListAsync(cancellationToken);
    }
}
