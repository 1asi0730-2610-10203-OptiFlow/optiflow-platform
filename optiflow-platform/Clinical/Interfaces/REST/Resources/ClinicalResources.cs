namespace optiflow_platform.Clinical.Interfaces.REST.Resources;

/// <summary>Resource returned by the API for a patient.</summary>
public record PatientResource(
    int     Id,
    string  FirstName,
    string  LastName,
    string  Dni,
    string? Phone,
    string? Email,
    string  BirthDate
);

/// <summary>Resource used to create a new patient.</summary>
public record CreatePatientResource(
    string  FirstName,
    string  LastName,
    string  Dni,
    string? Phone,
    string? Email,
    string  BirthDate
);

/// <summary>Resource used to update an existing patient.</summary>
public record UpdatePatientResource(
    string  FirstName,
    string  LastName,
    string  Dni,
    string? Phone,
    string? Email,
    string  BirthDate
);

/// <summary>Resource returned by the API for a clinical record.</summary>
public record ClinicalRecordResource(
    int Id,
    int PatientId
);

/// <summary>Resource returned by the API for a prescription.</summary>
public record PrescriptionResource(
    int      Id,
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

/// <summary>Resource used to create a new prescription.</summary>
public record CreatePrescriptionResource(
    int      ClinicalRecordId,
    decimal  OdSphere,
    decimal  OdCylinder,
    int      OdAxis,
    decimal  OiSphere,
    decimal  OiCylinder,
    int      OiAxis,
    decimal? Addition,
    string   Notes,
    string   DoctorName
);
