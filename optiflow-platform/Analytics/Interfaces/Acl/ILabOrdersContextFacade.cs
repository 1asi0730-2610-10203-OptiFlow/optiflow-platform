namespace optiflow_platform.Analytics.Interfaces.Acl;

/// <summary>
///     A minimal snapshot of a work order, exposed to the Analytics context for reporting.
/// </summary>
public record WorkOrderSummary(int SaleId, bool IsRework);

/// <summary>
///     Anti-Corruption Layer facade exposing only what the Analytics context needs from LabAndOrders.
/// </summary>
public interface ILabOrdersContextFacade
{
    /// <summary>
    ///     Returns a summary of every work order in the system.
    /// </summary>
    Task<IReadOnlyList<WorkOrderSummary>> FetchAllWorkOrdersAsync(CancellationToken cancellationToken = default);
}
