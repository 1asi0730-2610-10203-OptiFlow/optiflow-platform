namespace optiflow_platform.Sales.Interfaces.Acl;

/// <summary>
///     Anti-Corruption Layer facade exposing only what the Sales context needs from LabAndOrders.
/// </summary>
public interface ILabAndOrdersContextFacade
{
    /// <summary>
    ///     Returns the status of the work order linked to the given sale,
    ///     or null if no work order exists for that sale.
    /// </summary>
    Task<string?> FetchWorkOrderStatusBySaleId(int saleId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Returns the Inventory product ids (frame/lens) already consumed by the work order
    ///     linked to the given sale, or an empty set if no work order exists for that sale.
    /// </summary>
    Task<IReadOnlySet<int>> FetchWorkOrderMaterialProductIdsBySaleId(int saleId,
        CancellationToken cancellationToken = default);
}
