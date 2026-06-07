using optiflow_platform.Inventory.Domain.Model.Aggregates;
using optiflow_platform.Inventory.Domain.Repositories;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace optiflow_platform.Inventory.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     Entity Framework repository for product persistence.
/// </summary>
/// <param name="context">The EF Core database context.</param>
public class ProductRepository(AppDbContext context)
    : BaseRepository<Product>(context), IProductRepository
{
    /// <inheritdoc />
    public async Task<bool> ExistsBySkuAsync(string sku, CancellationToken cancellationToken = default) =>
        await Context.Set<Product>().AnyAsync(p => p.Sku == sku, cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<Product>> ListLowStockAsync(CancellationToken cancellationToken = default) =>
        await Context.Set<Product>()
            .Where(p => p.Stock <= p.MinimumStockThreshold)
            .ToListAsync(cancellationToken);
}
