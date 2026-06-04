using optiflow_platform.LabAndOrders.Domain.Model.Aggregates;
using optiflow_platform.LabAndOrders.Interfaces.REST.Resources;

namespace optiflow_platform.LabAndOrders.Interfaces.REST.Transform;

/// <summary>
///     Assembles a WorkOrderResource from a WorkOrder aggregate.
/// </summary>
public static class WorkOrderResourceFromEntityAssembler
{
    /// <summary>
    ///     Converts a WorkOrder entity to a WorkOrderResource.
    /// </summary>
    public static WorkOrderResource ToResourceFromEntity(WorkOrder workOrder) =>
        new(workOrder.Id, workOrder.SaleId, workOrder.RecipeId, workOrder.LabId,
            workOrder.Status, workOrder.Priority, workOrder.PatientName,
            workOrder.LaboratoryName, workOrder.LensType, workOrder.Frame,
            workOrder.Prescription, workOrder.DeliveryDate,
            workOrder.Deposit, workOrder.Total, workOrder.IsRework);
}
