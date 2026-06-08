using optiflow_platform.Analytics.Domain.Model.Aggregates;
using optiflow_platform.Analytics.Domain.Model.Entities;
using optiflow_platform.Analytics.Interfaces.REST.Resources;

namespace optiflow_platform.Analytics.Interfaces.REST.Transform;

/// <summary>
///     Assembler that converts an <see cref="AnalyticsReport"/> domain entity
///     into an <see cref="AnalyticsReportResource"/> REST resource.
/// </summary>
public static class AnalyticsReportResourceFromEntityAssembler
{
    /// <summary>Converts an <see cref="AnalyticsReport"/> to an <see cref="AnalyticsReportResource"/>.</summary>
    public static AnalyticsReportResource ToResourceFromEntity(AnalyticsReport entity) =>
        new(
            entity.Id,
            entity.GeneratedBy,
            entity.Period,
            entity.GeneratedAt.ToString("o"),
            entity.TotalRevenue,
            entity.TotalTransactions,
            entity.ConversionRate,
            entity.AverageDeliveryDays,
            entity.OnTimeDeliveryRate,
            entity.ReworkRate,
            entity.TotalOrders,
            entity.PendingBalance0To7,
            entity.PendingBalance8To15,
            entity.PendingBalance16To30,
            entity.PendingBalanceOver30
        );
}

/// <summary>
///     Assembler that converts a <see cref="StaffMetric"/> domain entity
///     into a <see cref="StaffMetricResource"/> REST resource.
/// </summary>
public static class StaffMetricResourceFromEntityAssembler
{
    /// <summary>Converts a <see cref="StaffMetric"/> to a <see cref="StaffMetricResource"/>.</summary>
    public static StaffMetricResource ToResourceFromEntity(StaffMetric entity) =>
        new(
            entity.Id,
            entity.ReportId,
            entity.EmployeeName,
            entity.QuotationsIssued,
            entity.SalesClosed,
            entity.TotalRevenue
        );
}
