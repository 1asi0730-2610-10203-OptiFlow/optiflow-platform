using optiflow_platform.Inventory.Domain.Model.Aggregates;
using optiflow_platform.Inventory.Interfaces.REST.Resources;

namespace optiflow_platform.Inventory.Interfaces.REST.Transform;

/// <summary>
///     Assembles a ProductResource from a Product aggregate.
/// </summary>
public static class ProductResourceFromEntityAssembler
{
    /// <summary>
    ///     Converts a Product entity to a ProductResource.
    /// </summary>
    public static ProductResource ToResourceFromEntity(Product product) =>
        new(product.Id, product.CategoryId, product.Category, product.SupplierId, product.SupplierName,
            product.Sku, product.Name, product.Brand, product.Model,
            product.Price, product.Stock, product.MinimumStockThreshold, product.LastRestockDate);
}
