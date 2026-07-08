using optiflow_platform.Shared.Domain.Model.Events;

namespace optiflow_platform.LabAndOrders.Domain.Model.Events;

/// <summary>
///     Raised after a work order is successfully created.
/// </summary>
/// <param name="WorkOrderId">The id of the newly created work order.</param>
/// <param name="LensProductId">
///     Reference to the Inventory product consumed as lens material, or null if none was selected.
/// </param>
/// <param name="FrameProductId">
///     Reference to the Inventory product consumed as frame material, or null if none was selected.
/// </param>
/// <param name="PatientName">Full name of the patient the order belongs to.</param>
/// <param name="AccountId">The account the work order belongs to, needed by cross-context handlers to scope their own queries.</param>
public record WorkOrderCreatedEvent(int WorkOrderId, int? LensProductId, int? FrameProductId, string PatientName, Guid AccountId) : IEvent;
