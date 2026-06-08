using optiflow_platform.Inventory.Domain.Model.Commands;
using optiflow_platform.Inventory.Interfaces.REST.Resources;

namespace optiflow_platform.Inventory.Interfaces.REST.Transform;

/// <summary>
///     Assembles an UpdateProductCommand from a product id and an UpdateProductResource.
/// </summary>
public static class UpdateProductCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts a product id and an UpdateProductResource to an UpdateProductCommand.
    /// </summary>
    public static UpdateProductCommand ToCommandFromResource(int productId, UpdateProductResource resource) =>
        new(productId, resource.Name, resource.Sku, resource.Category, resource.Price, resource.MinimumStockThreshold);
}
