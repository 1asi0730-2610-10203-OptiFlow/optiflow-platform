using optiflow_platform.Clinical.Domain.Model.Aggregates;
using optiflow_platform.Clinical.Domain.Repositories;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace optiflow_platform.Clinical.Infrastructure.Persistence.EFC.Repositories;

/// <summary>EF Core implementation of <see cref="IPatientRepository"/>.</summary>
public class PatientRepository(AppDbContext context)
    : BaseRepository<Patient>(context), IPatientRepository
{
    /// <inheritdoc />
    public async Task<Patient?> FindByDniAsync(string dni, CancellationToken cancellationToken = default) =>
        await Context.Set<Patient>()
            .FirstOrDefaultAsync(p => p.Dni == dni, cancellationToken);
}
