using optiflow_platform.Sales.Domain.Model.ValueObjects;

namespace optiflow_platform.Sales.Domain.Model.Commands;

public record CreateSaleCommand(
    InvoiceNumber InvoiceNumber,
    string? LabOrderNumber,
    int PatientId,
    string PatientName,
    int UserId,
    string UserName,
    decimal TotalAmount,
    decimal Advance,
    string? DiscountCode,
    decimal DiscountAmount,
    string PaymentMethod,
    string CreatedAt,
    string? DeliveredAt,
    string? Notes,
    IReadOnlyList<CreateSaleItemCommand> Items);

/// <summary>
///     A single product and quantity to record as part of a new sale.
/// </summary>
public record CreateSaleItemCommand(int ProductId, int Quantity);
