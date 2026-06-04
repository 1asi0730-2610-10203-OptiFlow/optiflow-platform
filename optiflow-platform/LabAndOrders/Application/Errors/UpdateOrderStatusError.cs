namespace optiflow_platform.LabAndOrders.Application.Errors;

/// <summary>
///     Represents errors that can occur when updating a work order status.
/// </summary>
public enum UpdateOrderStatusError
{
    /// <summary>
    ///     The work order with the given identifier was not found.
    /// </summary>
    WorkOrderNotFound,

    /// <summary>
    ///     The provided status value is invalid.
    /// </summary>
    InvalidStatus,

    /// <summary>
    ///     An unexpected error occurred during the operation.
    /// </summary>
    UnexpectedError
}
