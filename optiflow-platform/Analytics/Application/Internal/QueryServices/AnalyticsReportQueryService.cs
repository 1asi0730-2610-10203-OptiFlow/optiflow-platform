using optiflow_platform.Analytics.Application.Services;
using optiflow_platform.Analytics.Domain.Model.Aggregates;
using optiflow_platform.Analytics.Domain.Model.Queries;
using optiflow_platform.Analytics.Domain.Repositories;

namespace optiflow_platform.Analytics.Application.Internal.QueryServices;

/// <summary>
///     Query service implementation for <see cref="AnalyticsReport"/> read operations.
/// </summary>
/// <param name="analyticsReportRepository">The analytics report repository.</param>
/// <param name="logger">The logger instance.</param>
public class AnalyticsReportQueryService(
    IAnalyticsReportRepository analyticsReportRepository,
    ILogger<AnalyticsReportQueryService> logger)
    : IAnalyticsReportQueryService
{
    /// <inheritdoc />
    public async Task<IEnumerable<AnalyticsReport>> Handle(
        GetAllAnalyticsReportsQuery query,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Handling GetAllAnalyticsReportsQuery");
        return await analyticsReportRepository.ListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<AnalyticsReport?> Handle(
        GetAnalyticsReportByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Handling GetAnalyticsReportByIdQuery for id {Id}", query.Id);
        return await analyticsReportRepository.FindByIdAsync(query.Id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<AnalyticsReport>> Handle(
        GetAnalyticsReportsByPeriodQuery query,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Handling GetAnalyticsReportsByPeriodQuery for period {Period}", query.Period);
        return await analyticsReportRepository.FindByPeriodAsync(query.Period, cancellationToken);
    }
}
