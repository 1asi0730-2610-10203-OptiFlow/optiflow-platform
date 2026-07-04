namespace optiflow_platform.LabAndOrders.Application.Errors;

/// <summary>
///     Represents errors that can occur when linking a work order to a sale.
/// </summary>
public enum LinkSaleError
{
    /// <summary>
    ///     The work order with the given identifier was not found.
    /// </summary>
    WorkOrderNotFound,

    /// <summary>
    ///     An unexpected error occurred during the operation.
    /// </summary>
    UnexpectedError
}
