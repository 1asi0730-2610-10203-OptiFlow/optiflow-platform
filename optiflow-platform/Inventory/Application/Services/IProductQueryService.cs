using optiflow_platform.Inventory.Domain.Model.Aggregates;
using optiflow_platform.Inventory.Domain.Model.Queries;

namespace optiflow_platform.Inventory.Application.Services;

/// <summary>
///     Contract for product query operations.
/// </summary>
public interface IProductQueryService
{
    /// <summary>Returns all products in the catalog.</summary>
    Task<IEnumerable<Product>> Handle(GetAllProductsQuery query, CancellationToken cancellationToken = default);

    /// <summary>Returns a single product by its identifier.</summary>
    Task<Product?> Handle(GetProductByIdQuery query, CancellationToken cancellationToken = default);

    /// <summary>Returns the products whose stock is at or below their minimum stock threshold.</summary>
    Task<IEnumerable<Product>> Handle(GetLowStockProductsQuery query, CancellationToken cancellationToken = default);
}
