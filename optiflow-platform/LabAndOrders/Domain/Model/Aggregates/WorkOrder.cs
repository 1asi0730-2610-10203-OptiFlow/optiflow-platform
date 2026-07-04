using optiflow_platform.LabAndOrders.Domain.Model.Commands;
using optiflow_platform.LabAndOrders.Domain.Model.ValueObjects;

namespace optiflow_platform.LabAndOrders.Domain.Model.Aggregates;

/// <summary>
///     Work order aggregate root representing a lab production order.
/// </summary>
/// <remarks>
///     A work order tracks the full lifecycle of an optical product from creation
///     through production, quality control, and delivery.
///     Status transitions follow the flow: PENDING → IN_PRODUCTION → QUALITY_CONTROL → READY → DELIVERED.
///     Backward transitions are automatically flagged as rework.
/// </remarks>
public class WorkOrder
{
    private static readonly string[] StatusFlow =
    [
        OrderStatus.Pending,
        OrderStatus.InProduction,
        OrderStatus.QualityControl,
        OrderStatus.Ready,
        OrderStatus.Delivered
    ];

    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected WorkOrder()
    {
        LaboratoryId = null!;
        Status = null!;
        Priority = null!;
        PatientName = null!;
        LaboratoryName = null!;
        LensType = null!;
        Frame = null!;
        Prescription = null!;
        DeliveryDate = null!;
    }

    /// <summary>
    ///     Creates a new work order from a creation command.
    /// </summary>
    /// <param name="command">The CreateWorkOrderCommand command.</param>
    /// <exception cref="ArgumentNullException">Thrown when command is null.</exception>
    public WorkOrder(CreateWorkOrderCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        SaleId = command.SaleId;
        RecipeId = command.RecipeId;
        LaboratoryId = new LaboratoryId(command.LabId);
        Status = OrderStatus.Pending;
        Priority = command.Priority;
        PatientName = command.PatientName;
        LaboratoryName = command.LaboratoryName;
        LensType = command.LensType;
        LensProductId = command.LensProductId;
        Frame = command.Frame;
        FrameProductId = command.FrameProductId;
        Prescription = command.Prescription;
        DeliveryDate = command.DeliveryDate;
        Deposit = command.Deposit;
        Total = command.Total;
        IsRework = false;
    }

    public int Id { get; private set; }
    public int SaleId { get; private set; }
    public int RecipeId { get; private set; }
    public LaboratoryId LaboratoryId { get; private set; }
    public string Status { get; private set; }
    public string Priority { get; private set; }
    public string PatientName { get; private set; }
    public string LaboratoryName { get; private set; }
    public string LensType { get; private set; }
    public int? LensProductId { get; private set; }
    public string Frame { get; private set; }
    public int? FrameProductId { get; private set; }
    public string Prescription { get; private set; }
    public string DeliveryDate { get; private set; }
    public decimal Deposit { get; private set; }
    public decimal Total { get; private set; }
    public bool IsRework { get; private set; }

    /// <summary>
    ///     Updates the work order status. Backward transitions mark the order as rework.
    /// </summary>
    /// <param name="command">The UpdateOrderStatusCommand command.</param>
    /// <exception cref="ArgumentNullException">Thrown when command is null.</exception>
    public void UpdateStatus(UpdateOrderStatusCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        var previousStatus = Status;
        Status = command.Status;
        if (IsBackwardTransition(previousStatus, command.Status))
            IsRework = true;
    }

    /// <summary>
    ///     Links this work order to the sale created for it.
    /// </summary>
    public void LinkSale(int saleId)
    {
        SaleId = saleId;
    }

    private static bool IsBackwardTransition(string from, string to)
    {
        var fromIndex = Array.IndexOf(StatusFlow, from);
        var toIndex = Array.IndexOf(StatusFlow, to);
        return fromIndex >= 0 && toIndex >= 0 && toIndex < fromIndex;
    }
}
