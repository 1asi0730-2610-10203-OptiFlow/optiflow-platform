namespace optiflow_platform.Clinical.Domain.Model.Queries;

/// <summary>Query to retrieve all patients.</summary>
public record GetAllPatientsQuery;

/// <summary>Query to retrieve a patient by its identifier.</summary>
public record GetPatientByIdQuery(int Id);

/// <summary>Query to retrieve a patient by their DNI number.</summary>
public record GetPatientByDniQuery(string Dni);

/// <summary>Query to retrieve all clinical records.</summary>
public record GetAllClinicalRecordsQuery;

/// <summary>Query to retrieve the clinical record for a specific patient.</summary>
public record GetClinicalRecordByPatientIdQuery(int PatientId);

/// <summary>Query to retrieve a clinical record by its own identifier.</summary>
public record GetClinicalRecordByIdQuery(int Id);

/// <summary>Query to retrieve all prescriptions.</summary>
public record GetAllPrescriptionsQuery;

/// <summary>Query to retrieve all prescriptions belonging to a clinical record.</summary>
public record GetPrescriptionsByRecordIdQuery(int ClinicalRecordId);
