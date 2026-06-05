using optiflow_platform.Analytics.Domain.Model.Entities;
using optiflow_platform.Shared.Domain.Repositories;

namespace optiflow_platform.Analytics.Domain.Repositories;

/// <summary>
///     Repository contract for <see cref="StaffMetric"/> entities.
/// </summary>
public interface IStaffMetricRepository : IBaseRepository<StaffMetric>
{
    /// <summary>Returns all staff metrics that belong to the given report.</summary>
    Task<IEnumerable<StaffMetric>> FindByReportIdAsync(int reportId, CancellationToken cancellationToken = default);
}
