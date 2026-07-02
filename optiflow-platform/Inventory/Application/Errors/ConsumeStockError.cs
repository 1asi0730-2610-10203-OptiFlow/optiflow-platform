namespace optiflow_platform.Inventory.Application.Errors;

/// <summary>
///     Represents errors that can occur when consuming a product's stock.
/// </summary>
public enum ConsumeStockError
{
    /// <summary>
    ///     The product with the given identifier was not found.
    /// </summary>
    ProductNotFound,

    /// <summary>
    ///     The provided consumption quantity is zero or negative.
    /// </summary>
    InvalidQuantity,

    /// <summary>
    ///     The product does not have enough stock to satisfy the consumption.
    /// </summary>
    InsufficientStock,

    /// <summary>
    ///     An unexpected error occurred during the operation.
    /// </summary>
    UnexpectedError
}
