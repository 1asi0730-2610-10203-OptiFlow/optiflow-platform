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
    string? Notes);
