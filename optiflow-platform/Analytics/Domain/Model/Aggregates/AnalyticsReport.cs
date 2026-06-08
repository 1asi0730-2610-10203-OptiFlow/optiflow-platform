namespace optiflow_platform.Analytics.Domain.Model.Aggregates;

/// <summary>
///     Analytics report aggregate root representing a generated performance report for a period.
/// </summary>
/// <remarks>
///     An analytics report consolidates revenue, transaction, delivery, and order metrics
///     for a given reporting period (e.g. "2026-05"). Reports are generated and stored;
///     they are never modified after creation.
/// </remarks>
public class AnalyticsReport
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected AnalyticsReport()
    {
        GeneratedBy = null!;
        Period      = null!;
    }

    /// <summary>
    ///     Creates a new analytics report.
    /// </summary>
    public AnalyticsReport(
        string generatedBy,
        string period,
        decimal totalRevenue,
        int totalTransactions,
        decimal conversionRate,
        decimal averageDeliveryDays,
        decimal onTimeDeliveryRate,
        decimal reworkRate,
        int totalOrders,
        decimal pendingBalance0To7,
        decimal pendingBalance8To15,
        decimal pendingBalance16To30,
        decimal pendingBalanceOver30)
    {
        GeneratedBy           = generatedBy;
        Period                = period;
        GeneratedAt           = DateTime.UtcNow;
        TotalRevenue          = totalRevenue;
        TotalTransactions     = totalTransactions;
        ConversionRate        = conversionRate;
        AverageDeliveryDays   = averageDeliveryDays;
        OnTimeDeliveryRate    = onTimeDeliveryRate;
        ReworkRate            = reworkRate;
        TotalOrders           = totalOrders;
        PendingBalance0To7    = pendingBalance0To7;
        PendingBalance8To15   = pendingBalance8To15;
        PendingBalance16To30  = pendingBalance16To30;
        PendingBalanceOver30  = pendingBalanceOver30;
    }

    public int      Id                   { get; private set; }
    public string   GeneratedBy          { get; private set; }
    public string   Period               { get; private set; }
    public DateTime GeneratedAt          { get; private set; }
    public decimal  TotalRevenue         { get; private set; }
    public int      TotalTransactions    { get; private set; }
    public decimal  ConversionRate       { get; private set; }
    public decimal  AverageDeliveryDays  { get; private set; }
    public decimal  OnTimeDeliveryRate   { get; private set; }
    public decimal  ReworkRate           { get; private set; }
    public int      TotalOrders          { get; private set; }
    public decimal  PendingBalance0To7   { get; private set; }
    public decimal  PendingBalance8To15  { get; private set; }
    public decimal  PendingBalance16To30 { get; private set; }
    public decimal  PendingBalanceOver30 { get; private set; }
}
