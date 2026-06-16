using Microsoft.EntityFrameworkCore;
using optiflow_platform.PatientCenter.Domain.Model.Entities;
using optiflow_platform.PatientCenter.Domain.Repositories;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace optiflow_platform.PatientCenter.Infrastructure.Persistence.EFC.Repositories;

public class PatientNotificationRepository(AppDbContext context)
    : BaseRepository<PatientNotification>(context), IPatientNotificationRepository
{
    public async Task<IEnumerable<PatientNotification>> FindByPatientIdAsync(
        int patientId, CancellationToken cancellationToken = default) =>
        await Context.Set<PatientNotification>()
            .Where(n => n.PatientId == patientId)
            .ToListAsync(cancellationToken);
}