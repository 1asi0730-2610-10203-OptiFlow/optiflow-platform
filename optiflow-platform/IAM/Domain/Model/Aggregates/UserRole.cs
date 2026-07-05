namespace optiflow_platform.IAM.Domain.Model.Aggregates;

/// <summary>
///     Distinguishes the kind of account holder. <c>Admin</c> owns and runs an optic (business
///     dashboard); <c>Client</c> is a patient of an optic who uses the patient portal.
/// </summary>
public enum UserRole
{
    Admin,
    Client
}
