using optiflow_platform.Analytics.Domain.Model.Entities;
using optiflow_platform.Analytics.Domain.Model.Queries;

namespace optiflow_platform.Analytics.Application.Services;

/// <summary>
///     Query service contract for <see cref="StaffMetric"/> read operations.
/// </summary>
public interface IStaffMetricQueryService
{
    /// <summary>Returns all staff metrics.</summary>
    Task<IEnumerable<StaffMetric>> Handle(GetAllStaffMetricsQuery query, CancellationToken cancellationToken = default);

    /// <summary>Returns all staff metrics for a given report id.</summary>
    Task<IEnumerable<StaffMetric>> Handle(GetStaffMetricsByReportIdQuery query, CancellationToken cancellationToken = default);
}
