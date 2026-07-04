using optiflow_platform.Analytics.Application.Services;
using optiflow_platform.Analytics.Domain.Model.Aggregates;
using optiflow_platform.Analytics.Domain.Model.Queries;
using optiflow_platform.Analytics.Interfaces.Acl;

namespace optiflow_platform.Analytics.Application.Internal.QueryServices;

/// <summary>
///     Query service implementation for <see cref="AnalyticsReport"/> read operations.
/// </summary>
/// <remarks>
///     Reports are computed live, grouped by month, from real Sales and LabAndOrders data —
///     there is no write path that persists <see cref="AnalyticsReport"/> rows, so reading
///     them from the database would always return an empty set.
/// </remarks>
/// <param name="salesContextFacade">ACL facade for reading sale data.</param>
/// <param name="labOrdersContextFacade">ACL facade for reading work order data.</param>
/// <param name="logger">The logger instance.</param>
public class AnalyticsReportQueryService(
    ISalesContextFacade salesContextFacade,
    ILabOrdersContextFacade labOrdersContextFacade,
    ILogger<AnalyticsReportQueryService> logger)
    : IAnalyticsReportQueryService
{
    /// <inheritdoc />
    public async Task<IEnumerable<AnalyticsReport>> Handle(
        GetAllAnalyticsReportsQuery query,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Handling GetAllAnalyticsReportsQuery");
        return await BuildReportsAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<AnalyticsReport?> Handle(
        GetAnalyticsReportByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Handling GetAnalyticsReportByIdQuery for id {Id}", query.Id);
        var reports = await BuildReportsAsync(cancellationToken);
        return query.Id >= 1 && query.Id <= reports.Count ? reports[query.Id - 1] : null;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<AnalyticsReport>> Handle(
        GetAnalyticsReportsByPeriodQuery query,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Handling GetAnalyticsReportsByPeriodQuery for period {Period}", query.Period);
        var reports = await BuildReportsAsync(cancellationToken);
        return reports.Where(r => r.Period == query.Period);
    }

    private async Task<IReadOnlyList<AnalyticsReport>> BuildReportsAsync(CancellationToken cancellationToken)
    {
        var sales = await salesContextFacade.FetchAllSalesAsync(cancellationToken);
        var workOrders = await labOrdersContextFacade.FetchAllWorkOrdersAsync(cancellationToken);

        var salesByPeriod = sales
            .Where(s => !string.IsNullOrWhiteSpace(s.CreatedAt) && s.CreatedAt.Length >= 7)
            .GroupBy(s => PeriodOf(s.CreatedAt))
            .ToDictionary(g => g.Key, g => g.ToList());

        var periodBySaleId = sales.ToDictionary(s => s.Id, s => PeriodOf(s.CreatedAt));

        var orderCountByPeriod = workOrders
            .Where(w => periodBySaleId.ContainsKey(w.SaleId))
            .GroupBy(w => periodBySaleId[w.SaleId])
            .ToDictionary(g => g.Key, g => g.Count());

        var periods = salesByPeriod.Keys.Union(orderCountByPeriod.Keys).OrderBy(p => p).ToList();

        var reports = new List<AnalyticsReport>();
        foreach (var period in periods)
        {
            var periodSales = salesByPeriod.GetValueOrDefault(period, []);
            var report = new AnalyticsReport(
                generatedBy: "system",
                period: period,
                totalRevenue: periodSales.Sum(s => s.TotalAmount),
                totalTransactions: periodSales.Count,
                conversionRate: 0,
                averageDeliveryDays: 0,
                onTimeDeliveryRate: 0,
                reworkRate: 0,
                totalOrders: orderCountByPeriod.GetValueOrDefault(period, 0),
                pendingBalance0To7: 0,
                pendingBalance8To15: 0,
                pendingBalance16To30: 0,
                pendingBalanceOver30: 0);
            reports.Add(report);
        }

        return reports;

        static string PeriodOf(string createdAt) => createdAt[..7];
    }
}
