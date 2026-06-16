using optiflow_platform.Inventory.Domain.Model.Aggregates;
using optiflow_platform.Inventory.Interfaces.REST.Resources;

namespace optiflow_platform.Inventory.Interfaces.REST.Transform;

/// <summary>
///     Assembles a SupplierResource from a Supplier aggregate.
/// </summary>
public static class SupplierResourceFromEntityAssembler
{
    /// <summary>
    ///     Converts a Supplier aggregate to a SupplierResource.
    /// </summary>
    public static SupplierResource ToResourceFromEntity(Supplier supplier) =>
        new(supplier.Id, supplier.Name, supplier.Contact.ContactPerson, supplier.Contact.Phone, supplier.Contact.Email);
}
