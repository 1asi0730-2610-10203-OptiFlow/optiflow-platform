namespace optiflow_platform.LabAndOrders.Application.Errors;

/// <summary>
///     Represents errors that can occur when deleting a work order.
/// </summary>
public enum DeleteWorkOrderError
{
    /// <summary>
    ///     The work order with the given identifier was not found.
    /// </summary>
    WorkOrderNotFound,

    /// <summary>
    ///     The work order is not in the Delivered status, so it cannot be deleted.
    /// </summary>
    NotDelivered,

    /// <summary>
    ///     An unexpected error occurred during the operation.
    /// </summary>
    UnexpectedError
}
