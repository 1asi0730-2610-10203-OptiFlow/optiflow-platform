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

        // Only sales with a usable date participate; PeriodOf is null/length-safe so a single malformed
        // CreatedAt can no longer throw and 500 the whole endpoint.
        var datedSales = sales.Where(s => PeriodOf(s.CreatedAt) is not null).ToList();

        var salesByPeriod = datedSales
            .GroupBy(s => PeriodOf(s.CreatedAt)!)
            .ToDictionary(g => g.Key, g => g.ToList());

        var periodBySaleId = datedSales.ToDictionary(s => s.Id, s => PeriodOf(s.CreatedAt)!);

        var ordersByPeriod = workOrders
            .Where(w => periodBySaleId.ContainsKey(w.SaleId))
            .GroupBy(w => periodBySaleId[w.SaleId])
            .ToDictionary(g => g.Key, g => g.ToList());

        var periods = salesByPeriod.Keys.Union(ordersByPeriod.Keys).OrderBy(p => p).ToList();

        var reports = new List<AnalyticsReport>();
        foreach (var period in periods)
        {
            var periodSales = salesByPeriod.GetValueOrDefault(period, []);
            var periodOrders = ordersByPeriod.GetValueOrDefault(period, []);
            var totalOrders = periodOrders.Count;
            var reworked = periodOrders.Count(w => w.IsRework);

            decimal b0to7 = 0, b8to15 = 0, b16to30 = 0, bOver30 = 0;
            foreach (var s in periodSales)
            {
                if (s.PendingBalance <= 0) continue;
                var age = AgeInDays(s.CreatedAt);
                if (age <= 7) b0to7 += s.PendingBalance;
                else if (age <= 15) b8to15 += s.PendingBalance;
                else if (age <= 30) b16to30 += s.PendingBalance;
                else bOver30 += s.PendingBalance;
            }

            var report = new AnalyticsReport(
                generatedBy: "system",
                period: period,
                totalRevenue: periodSales.Sum(s => s.TotalAmount),
                totalTransactions: periodSales.Count,
                conversionRate: 0,
                averageDeliveryDays: 0,
                onTimeDeliveryRate: 0,
                reworkRate: totalOrders > 0 ? Math.Round((decimal)reworked / totalOrders * 100, 2) : 0,
                totalOrders: totalOrders,
                pendingBalance0To7: b0to7,
                pendingBalance8To15: b8to15,
                pendingBalance16To30: b16to30,
                pendingBalanceOver30: bOver30);
            reports.Add(report);
        }

        return reports;

        static string? PeriodOf(string? createdAt) =>
            string.IsNullOrWhiteSpace(createdAt) || createdAt.Length < 7 ? null : createdAt[..7];

        static int AgeInDays(string? createdAt) =>
            DateTimeOffset.TryParse(createdAt, out var dt)
                ? Math.Max(0, (int)(DateTimeOffset.UtcNow - dt).TotalDays)
                : 0;
    }
}
