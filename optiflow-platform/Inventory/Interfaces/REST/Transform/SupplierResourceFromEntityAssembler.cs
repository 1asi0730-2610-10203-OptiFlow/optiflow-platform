using optiflow_platform.Inventory.Domain.Model.Entities;
using optiflow_platform.Inventory.Interfaces.REST.Resources;

namespace optiflow_platform.Inventory.Interfaces.REST.Transform;

/// <summary>
///     Assembles a SupplierResource from a Supplier entity.
/// </summary>
public static class SupplierResourceFromEntityAssembler
{
    /// <summary>
    ///     Converts a Supplier entity to a SupplierResource.
    /// </summary>
    public static SupplierResource ToResourceFromEntity(Supplier supplier) =>
        new(supplier.Id, supplier.Name, supplier.ContactPerson, supplier.Phone, supplier.Email);
}
