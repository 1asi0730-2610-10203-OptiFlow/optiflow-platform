using optiflow_platform.Inventory.Domain.Model.Aggregates;
using optiflow_platform.Shared.Domain.Repositories;

namespace optiflow_platform.Inventory.Domain.Repositories;

/// <summary>
///     Repository contract for <see cref="Product"/> persistence.
/// </summary>
public interface IProductRepository : IBaseRepository<Product>
{
    /// <summary>
    ///     Determines whether a product with the given SKU already exists in the catalog.
    /// </summary>
    Task<bool> ExistsBySkuAsync(string sku, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Lists the products whose stock is at or below their minimum stock threshold.
    /// </summary>
    Task<IEnumerable<Product>> ListLowStockAsync(CancellationToken cancellationToken = default);
}
