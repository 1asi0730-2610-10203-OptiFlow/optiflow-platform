using optiflow_platform.Inventory.Domain.Model.Commands;
using optiflow_platform.Inventory.Interfaces.REST.Resources;

namespace optiflow_platform.Inventory.Interfaces.REST.Transform;

/// <summary>
///     Assembles a RegisterProductCommand from a RegisterProductResource.
/// </summary>
public static class RegisterProductCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts a RegisterProductResource to a RegisterProductCommand.
    /// </summary>
    /// <remarks>
    ///     The last restock date is set to the current date, since a newly registered
    ///     product is considered stocked at the moment of its registration.
    /// </remarks>
    public static RegisterProductCommand ToCommandFromResource(RegisterProductResource resource) =>
        new(resource.Category, resource.SupplierId, resource.SupplierName,
            resource.Sku, resource.Name, resource.Brand ?? string.Empty, resource.Model ?? string.Empty,
            resource.Price, resource.Stock, resource.MinimumStockThreshold,
            DateTimeOffset.UtcNow.ToString("yyyy-MM-dd"));
}
