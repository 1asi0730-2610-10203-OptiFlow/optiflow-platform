namespace optiflow_platform.Inventory.Application.Errors;

/// <summary>
///     Represents errors that can occur when restocking a product.
/// </summary>
public enum RestockProductError
{
    /// <summary>
    ///     The product with the given identifier was not found.
    /// </summary>
    ProductNotFound,

    /// <summary>
    ///     The provided restock quantity is zero or negative.
    /// </summary>
    InvalidQuantity,

    /// <summary>
    ///     An unexpected error occurred during the operation.
    /// </summary>
    UnexpectedError
}
