namespace optiflow_platform.Clinical.Application.Errors;

/// <summary>Possible errors when creating a patient.</summary>
public enum CreatePatientError
{
    DniAlreadyExists,
    UnexpectedError
}

/// <summary>Possible errors when updating a patient.</summary>
public enum UpdatePatientError
{
    PatientNotFound,
    DniAlreadyExists,
    UnexpectedError
}

/// <summary>Possible errors when creating a prescription.</summary>
public enum CreatePrescriptionError
{
    ClinicalRecordNotFound,
    UnexpectedError
}
