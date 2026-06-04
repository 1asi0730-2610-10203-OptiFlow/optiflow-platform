using optiflow_platform.LabAndOrders.Domain.Model.Commands;
using optiflow_platform.LabAndOrders.Interfaces.REST.Resources;

namespace optiflow_platform.LabAndOrders.Interfaces.REST.Transform;

/// <summary>
///     Assembles a CreateWorkOrderCommand from a CreateWorkOrderResource.
/// </summary>
public static class CreateWorkOrderCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts a CreateWorkOrderResource to a CreateWorkOrderCommand.
    /// </summary>
    public static CreateWorkOrderCommand ToCommandFromResource(CreateWorkOrderResource resource) =>
        new(resource.SaleId, resource.RecipeId, resource.LabId,
            resource.PatientName, resource.LaboratoryName ?? string.Empty,
            resource.LensType ?? string.Empty, resource.Frame ?? string.Empty,
            resource.Prescription ?? string.Empty,
            resource.Priority ?? "normal",
            resource.DeliveryDate,
            resource.Deposit, resource.Total);
}
