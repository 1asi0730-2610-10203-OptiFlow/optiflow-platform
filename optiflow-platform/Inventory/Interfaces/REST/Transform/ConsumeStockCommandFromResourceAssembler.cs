using optiflow_platform.Inventory.Domain.Model.Commands;
using optiflow_platform.Inventory.Interfaces.REST.Resources;

namespace optiflow_platform.Inventory.Interfaces.REST.Transform;

/// <summary>
///     Assembles a ConsumeStockCommand from a product id and a ConsumeStockResource.
/// </summary>
public static class ConsumeStockCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts a product id and a ConsumeStockResource to a ConsumeStockCommand.
    /// </summary>
    public static ConsumeStockCommand ToCommandFromResource(int productId, ConsumeStockResource resource) =>
        new(productId, resource.Quantity, resource.Author);
}
