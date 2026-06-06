namespace optiflow_platform.Clinical.Domain.Model.Commands;

/// <summary>Command to register a new patient.</summary>
public record CreatePatientCommand(
    string   FirstName,
    string   LastName,
    string   Dni,
    string?  Phone,
    string?  Email,
    DateOnly BirthDate);

/// <summary>Command to update an existing patient's personal data.</summary>
public record UpdatePatientCommand(
    int      PatientId,
    string   FirstName,
    string   LastName,
    string   Dni,
    string?  Phone,
    string?  Email,
    DateOnly BirthDate);

/// <summary>Command to add a new prescription to a clinical record.</summary>
public record CreatePrescriptionCommand(
    int      ClinicalRecordId,
    decimal  OdSphere,
    decimal  OdCylinder,
    int      OdAxis,
    decimal  OiSphere,
    decimal  OiCylinder,
    int      OiAxis,
    decimal? Addition,
    string   Notes,
    string   DoctorName);
