using Microsoft.EntityFrameworkCore;
using optiflow_platform.Sales.Domain.Model.Entities;
using optiflow_platform.Sales.Domain.Repositories;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace optiflow_platform.Sales.Infrastructure.Persistence.EFC.Repositories;

public class SaleItemRepository(AppDbContext context)
    : BaseRepository<SaleItem>(context), ISaleItemRepository
{
    public async Task<IEnumerable<SaleItem>> ListBySaleIdAsync(int saleId, CancellationToken cancellationToken = default) =>
        await Context.Set<SaleItem>().Where(i => i.SaleId == saleId).ToListAsync(cancellationToken);
}
