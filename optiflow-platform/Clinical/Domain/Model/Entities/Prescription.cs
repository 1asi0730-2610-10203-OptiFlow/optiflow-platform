using optiflow_platform.Clinical.Domain.Model.Commands;

namespace optiflow_platform.Clinical.Domain.Model.Entities;

/// <summary>
///     Prescription entity containing optical measurements for both eyes.
///     OD = right eye (Oculus Dexter), OI = left eye (Oculus Sinister).
/// </summary>
public class Prescription
{
    protected Prescription()
    {
        DoctorName       = null!;
        PrescriptionUuid = null!;
    }

    public Prescription(CreatePrescriptionCommand command, Guid accountId)
    {
        ArgumentNullException.ThrowIfNull(command);
        AccountId        = accountId;
        ClinicalRecordId = command.ClinicalRecordId;
        OdSphere         = command.OdSphere;
        OdCylinder       = command.OdCylinder;
        OdAxis           = command.OdAxis;
        OiSphere         = command.OiSphere;
        OiCylinder       = command.OiCylinder;
        OiAxis           = command.OiAxis;
        Addition         = command.Addition;
        Notes            = command.Notes;
        DoctorName       = command.DoctorName;
        CreatedAt        = DateTime.UtcNow;
        PrescriptionUuid = $"presc-{Guid.NewGuid():N}";
    }

    public int      Id               { get; private set; }
    public Guid     AccountId        { get; private set; }

    /// <summary>Alias of Id — kept for mockapi compatibility.</summary>
    public int      PrescriptionId   => Id;

    /// <summary>Stable UUID for external references.</summary>
    public string   PrescriptionUuid { get; private set; }

    public int      ClinicalRecordId { get; private set; }

    // Right eye (OD — Oculus Dexter)
    public decimal  OdSphere         { get; private set; }
    public decimal  OdCylinder       { get; private set; }
    public int      OdAxis           { get; private set; }

    // Left eye (OI — Oculus Sinister)
    public decimal  OiSphere         { get; private set; }
    public decimal  OiCylinder       { get; private set; }
    public int      OiAxis           { get; private set; }

    // Optional progressive addition
    public decimal? Addition         { get; private set; }

    public string   Notes            { get; private set; } = string.Empty;
    public string   DoctorName       { get; private set; }
    public DateTime CreatedAt        { get; private set; }
}