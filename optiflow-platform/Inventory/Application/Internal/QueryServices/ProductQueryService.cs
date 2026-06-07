using optiflow_platform.Inventory.Application.Services;
using optiflow_platform.Inventory.Domain.Model.Aggregates;
using optiflow_platform.Inventory.Domain.Model.Queries;
using optiflow_platform.Inventory.Domain.Repositories;

namespace optiflow_platform.Inventory.Application.Internal.QueryServices;

/// <summary>
///     Application service for handling product queries.
/// </summary>
/// <param name="productRepository">Repository for accessing product data.</param>
public class ProductQueryService(IProductRepository productRepository) : IProductQueryService
{
    /// <inheritdoc />
    public async Task<IEnumerable<Product>> Handle(GetAllProductsQuery query,
        CancellationToken cancellationToken = default) =>
        await productRepository.ListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<Product?> Handle(GetProductByIdQuery query, CancellationToken cancellationToken = default) =>
        await productRepository.FindByIdAsync(query.Id, cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<Product>> Handle(GetLowStockProductsQuery query,
        CancellationToken cancellationToken = default) =>
        await productRepository.ListLowStockAsync(cancellationToken);
}
