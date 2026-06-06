using optiflow_platform.Clinical.Application.Services;
using optiflow_platform.Clinical.Domain.Model.Aggregates;
using optiflow_platform.Clinical.Domain.Model.Entities;
using optiflow_platform.Clinical.Domain.Model.Queries;
using optiflow_platform.Clinical.Domain.Repositories;

namespace optiflow_platform.Clinical.Application.Internal.QueryServices;

/// <summary>Query service implementation for <see cref="Patient"/> read operations.</summary>
public class PatientQueryService(
    IPatientRepository patientRepository,
    ILogger<PatientQueryService> logger)
    : IPatientQueryService
{
    public async Task<IEnumerable<Patient>> Handle(GetAllPatientsQuery query, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Handling GetAllPatientsQuery");
        return await patientRepository.ListAsync(cancellationToken);
    }

    public async Task<Patient?> Handle(GetPatientByIdQuery query, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Handling GetPatientByIdQuery for id {Id}", query.Id);
        return await patientRepository.FindByIdAsync(query.Id, cancellationToken);
    }

    public async Task<Patient?> Handle(GetPatientByDniQuery query, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Handling GetPatientByDniQuery for dni {Dni}", query.Dni);
        return await patientRepository.FindByDniAsync(query.Dni, cancellationToken);
    }
}

/// <summary>Query service implementation for <see cref="ClinicalRecord"/> read operations.</summary>
public class ClinicalRecordQueryService(
    IClinicalRecordRepository clinicalRecordRepository,
    ILogger<ClinicalRecordQueryService> logger)
    : IClinicalRecordQueryService
{
    public async Task<IEnumerable<ClinicalRecord>> Handle(GetAllClinicalRecordsQuery query, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Handling GetAllClinicalRecordsQuery");
        return await clinicalRecordRepository.ListAsync(cancellationToken);
    }

    public async Task<ClinicalRecord?> Handle(GetClinicalRecordByIdQuery query, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Handling GetClinicalRecordByIdQuery for id {Id}", query.Id);
        return await clinicalRecordRepository.FindByIdAsync(query.Id, cancellationToken);
    }

    public async Task<ClinicalRecord?> Handle(GetClinicalRecordByPatientIdQuery query, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Handling GetClinicalRecordByPatientIdQuery for patientId {PatientId}", query.PatientId);
        return await clinicalRecordRepository.FindByPatientIdAsync(query.PatientId, cancellationToken);
    }
}

/// <summary>Query service implementation for <see cref="Prescription"/> read operations.</summary>
public class PrescriptionQueryService(
    IPrescriptionRepository prescriptionRepository,
    ILogger<PrescriptionQueryService> logger)
    : IPrescriptionQueryService
{
    public async Task<IEnumerable<Prescription>> Handle(GetAllPrescriptionsQuery query, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Handling GetAllPrescriptionsQuery");
        return await prescriptionRepository.ListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Prescription>> Handle(GetPrescriptionsByRecordIdQuery query, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Handling GetPrescriptionsByRecordIdQuery for recordId {RecordId}", query.ClinicalRecordId);
        return await prescriptionRepository.FindByClinicalRecordIdAsync(query.ClinicalRecordId, cancellationToken);
    }
}
