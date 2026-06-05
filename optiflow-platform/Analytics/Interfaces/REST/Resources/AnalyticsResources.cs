namespace optiflow_platform.Analytics.Interfaces.REST.Resources;

/// <summary>Resource returned by the API for an analytics report.</summary>
public record AnalyticsReportResource(
    int     Id,
    string  GeneratedBy,
    string  Period,
    string  GeneratedAt,
    decimal TotalRevenue,
    int     TotalTransactions,
    decimal ConversionRate,
    decimal AverageDeliveryDays,
    decimal OnTimeDeliveryRate,
    decimal ReworkRate,
    int     TotalOrders,
    decimal PendingBalance0To7,
    decimal PendingBalance8To15,
    decimal PendingBalance16To30,
    decimal PendingBalanceOver30
);

/// <summary>Resource returned by the API for a staff metric.</summary>
public record StaffMetricResource(
    int     Id,
    int     ReportId,
    string  EmployeeName,
    int     QuotationsIssued,
    int     SalesClosed,
    decimal TotalRevenue
);
