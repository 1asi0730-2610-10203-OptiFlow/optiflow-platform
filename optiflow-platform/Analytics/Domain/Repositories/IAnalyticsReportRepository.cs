using optiflow_platform.Analytics.Domain.Model.Aggregates;
using optiflow_platform.Shared.Domain.Repositories;

namespace optiflow_platform.Analytics.Domain.Repositories;

/// <summary>
///     Repository contract for <see cref="AnalyticsReport"/> aggregates.
/// </summary>
public interface IAnalyticsReportRepository : IBaseRepository<AnalyticsReport>
{
    /// <summary>Returns all reports whose period matches the given string (e.g. "2026-05").</summary>
    Task<IEnumerable<AnalyticsReport>> FindByPeriodAsync(string period, CancellationToken cancellationToken = default);
}
