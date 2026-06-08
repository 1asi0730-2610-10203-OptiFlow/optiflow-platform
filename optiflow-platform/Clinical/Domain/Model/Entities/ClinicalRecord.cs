namespace optiflow_platform.Clinical.Domain.Model.Entities;

/// <summary>
///     Clinical record entity that serves as the container for all prescriptions
///     belonging to a single patient.
/// </summary>
/// <remarks>
///     A clinical record is created automatically when a <see cref="Aggregates.Patient"/>
///     is registered. It has a 1-to-1 relationship with a patient and a 1-to-many
///     relationship with <see cref="Prescription"/>.
/// </remarks>
public class ClinicalRecord
{
    /// <summary>Protected parameterless constructor for EF Core.</summary>
    protected ClinicalRecord() { }

    /// <summary>Creates a new clinical record linked to the given patient.</summary>
    public ClinicalRecord(int patientId)
    {
        PatientId = patientId;
    }

    public int Id        { get; private set; }
    public int PatientId { get; private set; }

    private readonly List<Prescription> _prescriptions = [];

    /// <summary>Read-only view of all prescriptions attached to this record.</summary>
    public IReadOnlyCollection<Prescription> Prescriptions => _prescriptions.AsReadOnly();
}
