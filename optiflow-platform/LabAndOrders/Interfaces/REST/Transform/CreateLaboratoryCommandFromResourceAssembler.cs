using optiflow_platform.LabAndOrders.Domain.Model.Commands;
using optiflow_platform.LabAndOrders.Interfaces.REST.Resources;

namespace optiflow_platform.LabAndOrders.Interfaces.REST.Transform;

/// <summary>
///     Assembles a CreateLaboratoryCommand from a CreateLaboratoryResource.
/// </summary>
public static class CreateLaboratoryCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts a CreateLaboratoryResource to a CreateLaboratoryCommand.
    /// </summary>
    public static CreateLaboratoryCommand ToCommandFromResource(CreateLaboratoryResource resource) =>
        new(resource.Name, resource.Phone, resource.Email);
}
