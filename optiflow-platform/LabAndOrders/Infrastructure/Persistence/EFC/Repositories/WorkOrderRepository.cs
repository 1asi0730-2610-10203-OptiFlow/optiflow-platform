using optiflow_platform.LabAndOrders.Domain.Model.Aggregates;
using optiflow_platform.LabAndOrders.Domain.Repositories;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace optiflow_platform.LabAndOrders.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     Entity Framework repository for work order persistence.
/// </summary>
/// <param name="context">The EF Core database context.</param>
public class WorkOrderRepository(AppDbContext context)
    : BaseRepository<WorkOrder>(context), IWorkOrderRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<WorkOrder>> FindByStatusAsync(string status,
        CancellationToken cancellationToken = default) =>
        await Context.Set<WorkOrder>()
            .Where(w => w.Status == status)
            .ToListAsync(cancellationToken);
}
