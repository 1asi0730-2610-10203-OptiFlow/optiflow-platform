using optiflow_platform.Clinical.Application.Errors;
using optiflow_platform.Clinical.Domain.Model.Commands;
using optiflow_platform.Clinical.Domain.Model.Entities;
using optiflow_platform.Clinical.Domain.Model.Queries;
using optiflow_platform.Shared.Application.Patterns;

namespace optiflow_platform.Clinical.Application.Services;

/// <summary>Query service contract for read operations on <see cref="ClinicalRecord"/>.</summary>
public interface IClinicalRecordQueryService
{
    Task<IEnumerable<ClinicalRecord>> Handle(GetAllClinicalRecordsQuery query, CancellationToken cancellationToken = default);
    Task<ClinicalRecord?> Handle(GetClinicalRecordByIdQuery query, CancellationToken cancellationToken = default);
    Task<ClinicalRecord?> Handle(GetClinicalRecordByPatientIdQuery query, CancellationToken cancellationToken = default);
}

/// <summary>Command service contract for write operations on <see cref="Prescription"/>.</summary>
public interface IPrescriptionCommandService
{
    Task<Result<Prescription, CreatePrescriptionError>> Handle(CreatePrescriptionCommand command, CancellationToken cancellationToken = default);
}

/// <summary>Query service contract for read operations on <see cref="Prescription"/>.</summary>
public interface IPrescriptionQueryService
{
    Task<IEnumerable<Prescription>> Handle(GetAllPrescriptionsQuery query, CancellationToken cancellationToken = default);
    Task<IEnumerable<Prescription>> Handle(GetPrescriptionsByRecordIdQuery query, CancellationToken cancellationToken = default);
}
