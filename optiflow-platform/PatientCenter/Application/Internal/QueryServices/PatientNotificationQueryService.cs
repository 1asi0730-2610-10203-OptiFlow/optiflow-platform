using optiflow_platform.PatientCenter.Application.Services;
using optiflow_platform.PatientCenter.Domain.Model.Entities;
using optiflow_platform.PatientCenter.Domain.Model.Queries;
using optiflow_platform.PatientCenter.Domain.Repositories;

namespace optiflow_platform.PatientCenter.Application.Internal.QueryServices;

public class PatientNotificationQueryService(IPatientNotificationRepository notificationRepository)
    : IPatientNotificationQueryService
{
    public async Task<IEnumerable<PatientNotification>> Handle(GetNotificationsByPatientIdQuery query,
        CancellationToken cancellationToken = default) =>
        await notificationRepository.FindByPatientIdAsync(query.PatientId, cancellationToken);
}