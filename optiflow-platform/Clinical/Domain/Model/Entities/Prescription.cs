using optiflow_platform.Clinical.Domain.Model.Commands;

namespace optiflow_platform.Clinical.Domain.Model.Entities;

/// <summary>
///     Prescription entity containing the optical measurements for both eyes
///     captured during a clinical examination.
/// </summary>
/// <remarks>
///     A prescription belongs to a <see cref="ClinicalRecord"/>. OD = right eye (Oculus Dexter),
///     OI = left eye (Oculus Sinister). Addition is optional and applies to progressive lenses.
/// </remarks>
public class Prescription
{
    /// <summary>Protected parameterless constructor for EF Core.</summary>
    protected Prescription()
    {
        DoctorName = null!;
    }

    /// <summary>Creates a new prescription from a creation command.</summary>
    public Prescription(CreatePrescriptionCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
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
    }

    public int       Id               { get; private set; }
    public int       ClinicalRecordId { get; private set; }

    // Right eye (OD)
    public decimal  OdSphere   { get; private set; }
    public decimal  OdCylinder { get; private set; }
    public int      OdAxis     { get; private set; }

    // Left eye (OI)
    public decimal  OiSphere   { get; private set; }
    public decimal  OiCylinder { get; private set; }
    public int      OiAxis     { get; private set; }

    // Optional progressive addition
    public decimal?  Addition  { get; private set; }

    public string    Notes      { get; private set; } = string.Empty;
    public string    DoctorName { get; private set; }
    public DateTime  CreatedAt  { get; private set; }
}
