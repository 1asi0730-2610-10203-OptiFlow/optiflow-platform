using optiflow_platform.Inventory.Domain.Model.Commands;
using optiflow_platform.Inventory.Interfaces.REST.Resources;

namespace optiflow_platform.Inventory.Interfaces.REST.Transform;

/// <summary>
///     Assembles a CreateSupplierCommand from a RegisterSupplierResource.
/// </summary>
public static class RegisterSupplierCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts a RegisterSupplierResource to a CreateSupplierCommand.
    /// </summary>
    public static CreateSupplierCommand ToCommandFromResource(RegisterSupplierResource resource) =>
        new(resource.Name, resource.ContactPerson, resource.Phone, resource.Email);
}
