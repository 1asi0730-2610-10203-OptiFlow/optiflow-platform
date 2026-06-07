namespace optiflow_platform.Inventory.Application.Errors;

/// <summary>
///     Represents errors that can occur when verifying a product's stock for a supply request.
/// </summary>
public enum VerifySupplyStockError
{
    /// <summary>
    ///     The product with the given identifier was not found.
    /// </summary>
    ProductNotFound,

    /// <summary>
    ///     The product has no stock available to satisfy the supply request.
    /// </summary>
    OutOfStock,

    /// <summary>
    ///     An unexpected error occurred during the operation.
    /// </summary>
    UnexpectedError
}
