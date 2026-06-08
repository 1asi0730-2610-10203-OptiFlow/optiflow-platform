namespace optiflow_platform.Analytics.Domain.Model.Queries;

/// <summary>Query to retrieve all analytics reports.</summary>
public record GetAllAnalyticsReportsQuery;

/// <summary>Query to retrieve a single analytics report by its identifier.</summary>
/// <param name="Id">The report identifier.</param>
public record GetAnalyticsReportByIdQuery(int Id);

/// <summary>Query to retrieve analytics reports filtered by period (e.g. "2026-05").</summary>
/// <param name="Period">The period string in yyyy-MM format.</param>
public record GetAnalyticsReportsByPeriodQuery(string Period);

/// <summary>Query to retrieve all staff metrics.</summary>
public record GetAllStaffMetricsQuery;

/// <summary>Query to retrieve staff metrics for a specific report.</summary>
/// <param name="ReportId">The parent report identifier.</param>
public record GetStaffMetricsByReportIdQuery(int ReportId);
