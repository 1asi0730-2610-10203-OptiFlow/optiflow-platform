using optiflow_platform.LabAndOrders.Domain.Model.Commands;
using optiflow_platform.LabAndOrders.Interfaces.REST.Resources;

namespace optiflow_platform.LabAndOrders.Interfaces.REST.Transform;

/// <summary>
///     Assembles an UpdateOrderStatusCommand from a work order id and an UpdateWorkOrderStatusResource.
/// </summary>
public static class UpdateOrderStatusCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts a work order id and an UpdateWorkOrderStatusResource to an UpdateOrderStatusCommand.
    /// </summary>
    public static UpdateOrderStatusCommand ToCommandFromResource(int workOrderId, UpdateWorkOrderStatusResource resource) =>
        new(workOrderId, resource.Status);
}
