namespace optiflow_platform.Inventory.Application.Errors;

/// <summary>
///     Represents errors that can occur when registering a supplier.
/// </summary>
public enum CreateSupplierError
{
    /// <summary>A supplier with the same name already exists.</summary>
    DuplicateName,

    /// <summary>The provided contact information is invalid.</summary>
    InvalidContact,

    /// <summary>An unexpected error occurred during the operation.</summary>
    UnexpectedError
}
