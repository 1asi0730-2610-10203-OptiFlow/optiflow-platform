using System.ComponentModel.DataAnnotations;

namespace optiflow_platform.Clinical.Interfaces.REST.Resources;

/// <summary>Resource returned by the API for a patient.</summary>
public record PatientResource(
    int     Id,
    int     PatientId,
    string  CustomerUuid,
    string  FirstName,
    string  LastName,
    string  Dni,
    string? Phone,
    string? Email,
    string  BirthDate
);

/// <summary>Resource used to create a new patient.</summary>
public record CreatePatientResource(
    [Required] string  FirstName,
    [Required] string  LastName,
    [Required] string  Dni,
               string? Phone,
               string? Email,
    [Required] string  BirthDate
);

/// <summary>Resource used to update an existing patient.</summary>
public record UpdatePatientResource(
    [Required] string  FirstName,
    [Required] string  LastName,
    [Required] string  Dni,
               string? Phone,
               string? Email,
    [Required] string  BirthDate
);

/// <summary>Resource returned by the API for a clinical record.</summary>
public record ClinicalRecordResource(
    int    Id,
    int    RecordId,
    string ClinicalRecordUuid,
    int    PatientId
);

/// <summary>Resource returned by the API for a prescription.</summary>
public record PrescriptionResource(
    int      Id,
    int      PrescriptionId,
    string   PrescriptionUuid,
    int      ClinicalRecordId,
    decimal  OdSphere,
    decimal  OdCylinder,
    int      OdAxis,
    decimal  OiSphere,
    decimal  OiCylinder,
    int      OiAxis,
    decimal? Addition,
    string   Notes,
    string   DoctorName,
    string   CreatedAt
);

/// <summary>Resource used to create a new prescription — includes optical field validation.</summary>
public record CreatePrescriptionResource(
    [Required]
    int ClinicalRecordId,

    // ── Right eye (OD) ───────────────────────────────────────────────────
    [Required]
    [Range(-20.00, 20.00, ErrorMessage = "OdSphere must be between -20.00 and +20.00")]
    decimal OdSphere,

    [Required]
    [Range(-10.00, 10.00, ErrorMessage = "OdCylinder must be between -10.00 and +10.00")]
    decimal OdCylinder,

    [Required]
    [Range(0, 180, ErrorMessage = "OdAxis must be between 0 and 180")]
    int OdAxis,

    // ── Left eye (OI) ────────────────────────────────────────────────────
    [Required]
    [Range(-20.00, 20.00, ErrorMessage = "OiSphere must be between -20.00 and +20.00")]
    decimal OiSphere,

    [Required]
    [Range(-10.00, 10.00, ErrorMessage = "OiCylinder must be between -10.00 and +10.00")]
    decimal OiCylinder,

    [Required]
    [Range(0, 180, ErrorMessage = "OiAxis must be between 0 and 180")]
    int OiAxis,

    // ── Progressive addition (optional) ──────────────────────────────────
    [Range(0.00, 4.00, ErrorMessage = "Addition must be between 0.00 and +4.00")]
    decimal? Addition,

    string Notes,

    [Required]
    string DoctorName
);