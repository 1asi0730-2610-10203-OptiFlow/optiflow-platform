using optiflow_platform.Analytics.Application.Services;
using optiflow_platform.Analytics.Domain.Model.Entities;
using optiflow_platform.Analytics.Domain.Model.Queries;
using optiflow_platform.Analytics.Domain.Repositories;

namespace optiflow_platform.Analytics.Application.Internal.QueryServices;

/// <summary>
///     Query service implementation for <see cref="StaffMetric"/> read operations.
/// </summary>
/// <param name="staffMetricRepository">The staff metric repository.</param>
/// <param name="logger">The logger instance.</param>
public class StaffMetricQueryService(
    IStaffMetricRepository staffMetricRepository,
    ILogger<StaffMetricQueryService> logger)
    : IStaffMetricQueryService
{
    /// <inheritdoc />
    public async Task<IEnumerable<StaffMetric>> Handle(
        GetAllStaffMetricsQuery query,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Handling GetAllStaffMetricsQuery");
        return await staffMetricRepository.ListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<StaffMetric>> Handle(
        GetStaffMetricsByReportIdQuery query,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Handling GetStaffMetricsByReportIdQuery for reportId {ReportId}", query.ReportId);
        return await staffMetricRepository.FindByReportIdAsync(query.ReportId, cancellationToken);
    }
}
