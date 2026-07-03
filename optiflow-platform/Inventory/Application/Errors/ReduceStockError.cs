namespace optiflow_platform.Inventory.Application.Errors;

/// <summary>
///     Represents errors that can occur when reducing a product's stock.
/// </summary>
public enum ReduceStockError
{
    /// <summary>
    ///     The product with the given identifier was not found.
    /// </summary>
    ProductNotFound,

    /// <summary>
    ///     The provided reduction quantity is zero or negative.
    /// </summary>
    InvalidQuantity,

    /// <summary>
    ///     The requested quantity exceeds the product's available stock.
    /// </summary>
    InsufficientStock,

    /// <summary>
    ///     An unexpected error occurred during the operation.
    /// </summary>
    UnexpectedError
}
