using optiflow_platform.LabAndOrders.Domain.Model.Aggregates;
using optiflow_platform.LabAndOrders.Interfaces.REST.Resources;

namespace optiflow_platform.LabAndOrders.Interfaces.REST.Transform;

/// <summary>
///     Assembles a LaboratoryResource from a Laboratory entity.
/// </summary>
public static class LaboratoryResourceFromEntityAssembler
{
    /// <summary>
    ///     Converts a Laboratory entity to a LaboratoryResource.
    /// </summary>
    public static LaboratoryResource ToResourceFromEntity(Laboratory laboratory) =>
        new(laboratory.Id, laboratory.Name, laboratory.ContactInfo.Phone, laboratory.ContactInfo.Email);
}
