namespace optiflow_platform.Clinical.Domain.Model.Entities;

/// <summary>
///     Clinical record entity — container for all prescriptions of a single patient.
///     Created automatically when a Patient is registered.
/// </summary>
public class ClinicalRecord
{
    protected ClinicalRecord()
    {
        ClinicalRecordUuid = null!;
    }

    public ClinicalRecord(int patientId, Guid accountId)
    {
        PatientId          = patientId;
        AccountId          = accountId;
        ClinicalRecordUuid = $"record-{Guid.NewGuid():N}";
    }

    public int    Id        { get; private set; }
    public Guid   AccountId { get; private set; }

    /// <summary>Alias of Id — kept for mockapi compatibility.</summary>
    public int    RecordId  => Id;

    /// <summary>Stable UUID for external references.</summary>
    public string ClinicalRecordUuid { get; private set; }

    public int    PatientId { get; private set; }

    private readonly List<Prescription> _prescriptions = [];
    public IReadOnlyCollection<Prescription> Prescriptions => _prescriptions.AsReadOnly();
}