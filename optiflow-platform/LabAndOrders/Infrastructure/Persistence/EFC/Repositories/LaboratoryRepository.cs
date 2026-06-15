using Microsoft.EntityFrameworkCore;
using optiflow_platform.LabAndOrders.Domain.Model.Aggregates;
using optiflow_platform.LabAndOrders.Domain.Repositories;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace optiflow_platform.LabAndOrders.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     Entity Framework repository for laboratory persistence.
/// </summary>
/// <param name="context">The EF Core database context.</param>
public class LaboratoryRepository(AppDbContext context)
    : BaseRepository<Laboratory>(context), ILaboratoryRepository
{
    /// <inheritdoc />
    public async Task<Laboratory?> FindByNameAsync(string name, CancellationToken cancellationToken = default) =>
        await Context.Set<Laboratory>().FirstOrDefaultAsync(l => l.Name == name, cancellationToken);
}
