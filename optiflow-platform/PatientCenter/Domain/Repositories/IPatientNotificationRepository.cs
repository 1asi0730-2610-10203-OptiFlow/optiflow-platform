using optiflow_platform.PatientCenter.Domain.Model.Entities;
using optiflow_platform.Shared.Domain.Repositories;

namespace optiflow_platform.PatientCenter.Domain.Repositories;

public interface IPatientNotificationRepository : IBaseRepository<PatientNotification>
{
    Task<IEnumerable<PatientNotification>> FindByPatientIdAsync(
        int patientId, CancellationToken cancellationToken = default);
}