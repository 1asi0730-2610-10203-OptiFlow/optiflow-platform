using optiflow_platform.Inventory.Domain.Model.Commands;
using optiflow_platform.Inventory.Interfaces.REST.Resources;

namespace optiflow_platform.Inventory.Interfaces.REST.Transform;

/// <summary>
///     Assembles an UpdateStockLevelCommand from a product id and an UpdateStockLevelResource.
/// </summary>
public static class UpdateStockLevelCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts a product id and an UpdateStockLevelResource to an UpdateStockLevelCommand.
    /// </summary>
    public static UpdateStockLevelCommand ToCommandFromResource(int productId, UpdateStockLevelResource resource) =>
        new(productId, resource.NewStock);
}
