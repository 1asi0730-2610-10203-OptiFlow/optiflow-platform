namespace optiflow_platform.Inventory.Application.Errors;

/// <summary>
///     Represents errors that can occur when logging a manual stock adjustment.
/// </summary>
public enum LogManualAdjustmentError
{
    /// <summary>
    ///     The product with the given identifier was not found.
    /// </summary>
    ProductNotFound,

    /// <summary>
    ///     No justification was provided to confirm the adjustment.
    /// </summary>
    JustificationRequired,

    /// <summary>
    ///     An unexpected error occurred during the operation.
    /// </summary>
    UnexpectedError
}
