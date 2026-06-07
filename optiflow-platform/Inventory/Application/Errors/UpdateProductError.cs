namespace optiflow_platform.Inventory.Application.Errors;

/// <summary>
///     Represents errors that can occur when updating a product's catalog details.
/// </summary>
public enum UpdateProductError
{
    /// <summary>
    ///     The product with the given identifier was not found.
    /// </summary>
    ProductNotFound,

    /// <summary>
    ///     An unexpected error occurred during the operation.
    /// </summary>
    UnexpectedError
}
