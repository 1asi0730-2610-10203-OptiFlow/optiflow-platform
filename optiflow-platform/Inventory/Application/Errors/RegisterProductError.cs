namespace optiflow_platform.Inventory.Application.Errors;

/// <summary>
///     Represents errors that can occur when registering a product.
/// </summary>
public enum RegisterProductError
{
    /// <summary>
    ///     A product with the given SKU already exists in the catalog.
    /// </summary>
    DuplicateSku,

    /// <summary>
    ///     An unexpected error occurred during the operation.
    /// </summary>
    UnexpectedError
}
