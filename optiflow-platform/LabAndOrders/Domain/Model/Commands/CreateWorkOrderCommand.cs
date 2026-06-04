namespace optiflow_platform.LabAndOrders.Domain.Model.Commands;

/// <summary>
///     Command to create a new work order.
/// </summary>
public record CreateWorkOrderCommand(
    int SaleId,
    int RecipeId,
    int LabId,
    string PatientName,
    string LaboratoryName,
    string LensType,
    string Frame,
    string Prescription,
    string Priority,
    string DeliveryDate,
    decimal Deposit,
    decimal Total);
