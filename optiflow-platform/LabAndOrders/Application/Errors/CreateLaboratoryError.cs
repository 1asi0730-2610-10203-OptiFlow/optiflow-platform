namespace optiflow_platform.LabAndOrders.Application.Errors;

/// <summary>
///     Represents errors that can occur when registering a laboratory.
/// </summary>
public enum CreateLaboratoryError
{
    /// <summary>A laboratory with the same name already exists.</summary>
    DuplicateName,

    /// <summary>The provided contact information is invalid.</summary>
    InvalidContactInfo,

    /// <summary>An unexpected error occurred during the operation.</summary>
    UnexpectedError
}
