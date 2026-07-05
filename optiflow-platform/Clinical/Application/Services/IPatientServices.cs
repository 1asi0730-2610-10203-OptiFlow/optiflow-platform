using optiflow_platform.Clinical.Application.Errors;
using optiflow_platform.Clinical.Domain.Model.Aggregates;
using optiflow_platform.Clinical.Domain.Model.Commands;
using optiflow_platform.Clinical.Domain.Model.Queries;
using optiflow_platform.Shared.Application.Patterns;

namespace optiflow_platform.Clinical.Application.Services;

/// <summary>Command service contract for write operations on <see cref="Patient"/>.</summary>
public interface IPatientCommandService
{
    Task<Result<Patient, CreatePatientError>> Handle(CreatePatientCommand command, CancellationToken cancellationToken = default);
    Task<Result<Patient, UpdatePatientError>> Handle(UpdatePatientCommand command, CancellationToken cancellationToken = default);
}

/// <summary>Query service contract for read operations on <see cref="Patient"/>.</summary>
public interface IPatientQueryService
{
    Task<IEnumerable<Patient>> Handle(GetAllPatientsQuery query, CancellationToken cancellationToken = default);
    Task<Patient?> Handle(GetPatientByIdQuery query, CancellationToken cancellationToken = default);
    Task<Patient?> Handle(GetPatientByDniQuery query, CancellationToken cancellationToken = default);
    Task<Patient?> Handle(GetPatientByEmailQuery query, CancellationToken cancellationToken = default);
}
