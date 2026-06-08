using optiflow_platform.Clinical.Domain.Model.Entities;
using optiflow_platform.Clinical.Domain.Repositories;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace optiflow_platform.Clinical.Infrastructure.Persistence.EFC.Repositories;

/// <summary>EF Core implementation of <see cref="IClinicalRecordRepository"/>.</summary>
public class ClinicalRecordRepository(AppDbContext context)
    : BaseRepository<ClinicalRecord>(context), IClinicalRecordRepository
{
    /// <inheritdoc />
    public async Task<ClinicalRecord?> FindByPatientIdAsync(int patientId, CancellationToken cancellationToken = default) =>
        await Context.Set<ClinicalRecord>()
            .FirstOrDefaultAsync(r => r.PatientId == patientId, cancellationToken);
}

/// <summary>EF Core implementation of <see cref="IPrescriptionRepository"/>.</summary>
public class PrescriptionRepository(AppDbContext context)
    : BaseRepository<Prescription>(context), IPrescriptionRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<Prescription>> FindByClinicalRecordIdAsync(
        int clinicalRecordId,
        CancellationToken cancellationToken = default) =>
        await Context.Set<Prescription>()
            .Where(p => p.ClinicalRecordId == clinicalRecordId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
}
