using optiflow_platform.PatientCenter.Application.Errors;
using optiflow_platform.PatientCenter.Domain.Model.Commands;
using optiflow_platform.PatientCenter.Domain.Model.Entities;
using optiflow_platform.Shared.Application.Patterns;

namespace optiflow_platform.PatientCenter.Application.Services;

public interface IPatientNotificationCommandService
{
    Task<Result<PatientNotification, CreateNotificationError>> Handle(
        CreateNotificationCommand command,
        CancellationToken cancellationToken = default);
}