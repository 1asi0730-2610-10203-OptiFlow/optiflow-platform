using optiflow_platform.Inventory.Domain.Model.Commands;
using optiflow_platform.Inventory.Interfaces.REST.Resources;

namespace optiflow_platform.Inventory.Interfaces.REST.Transform;

/// <summary>
///     Assembles a RestockProductCommand from a product id and a RestockProductResource.
/// </summary>
public static class RestockProductCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts a product id and a RestockProductResource to a RestockProductCommand.
    /// </summary>
    public static RestockProductCommand ToCommandFromResource(int productId, RestockProductResource resource) =>
        new(productId, resource.Quantity, resource.Author);
}
