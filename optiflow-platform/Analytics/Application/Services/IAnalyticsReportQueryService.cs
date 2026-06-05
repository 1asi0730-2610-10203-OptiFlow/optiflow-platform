using optiflow_platform.Analytics.Domain.Model.Aggregates;
using optiflow_platform.Analytics.Domain.Model.Queries;

namespace optiflow_platform.Analytics.Application.Services;

/// <summary>
///     Query service contract for <see cref="AnalyticsReport"/> read operations.
/// </summary>
public interface IAnalyticsReportQueryService
{
    /// <summary>Returns all analytics reports.</summary>
    Task<IEnumerable<AnalyticsReport>> Handle(GetAllAnalyticsReportsQuery query, CancellationToken cancellationToken = default);

    /// <summary>Returns a single analytics report by id, or null if not found.</summary>
    Task<AnalyticsReport?> Handle(GetAnalyticsReportByIdQuery query, CancellationToken cancellationToken = default);

    /// <summary>Returns all analytics reports for a given period.</summary>
    Task<IEnumerable<AnalyticsReport>> Handle(GetAnalyticsReportsByPeriodQuery query, CancellationToken cancellationToken = default);
}
