namespace optiflow_platform.Analytics.Interfaces.Acl;

/// <summary>
///     A minimal snapshot of a sale, exposed to the Analytics context for reporting.
/// </summary>
public record SaleSummary(int Id, string CreatedAt, decimal TotalAmount, decimal PendingBalance, string Status);

/// <summary>
///     Anti-Corruption Layer facade exposing only what the Analytics context needs from Sales.
/// </summary>
public interface ISalesContextFacade
{
    /// <summary>
    ///     Returns a summary of every sale in the system.
    /// </summary>
    Task<IReadOnlyList<SaleSummary>> FetchAllSalesAsync(CancellationToken cancellationToken = default);
}
