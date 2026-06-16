using optiflow_platform.PatientCenter.Domain.Model.Entities;
using optiflow_platform.PatientCenter.Domain.Model.Queries;

namespace optiflow_platform.PatientCenter.Application.Services;

public interface IPatientNotificationQueryService
{
    Task<IEnumerable<PatientNotification>> Handle(GetNotificationsByPatientIdQuery query,
        CancellationToken cancellationToken = default);
}