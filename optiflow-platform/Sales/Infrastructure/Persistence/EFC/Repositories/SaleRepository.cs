using Microsoft.EntityFrameworkCore;
using optiflow_platform.Sales.Domain.Model.Aggregates;
using optiflow_platform.Sales.Domain.Model.ValueObjects;
using optiflow_platform.Sales.Domain.Repositories;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace optiflow_platform.Sales.Infrastructure.Persistence.EFC.Repositories;

public class SaleRepository(AppDbContext context)
    : BaseRepository<Sale>(context), ISaleRepository
{
    public override async Task<Sale?> FindByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await Context.Set<Sale>().FirstOrDefaultAsync(s => s.Id == new SaleId(id), cancellationToken);
}
