namespace optiflow_platform.Sales.Domain.Model.Commands;

public record CreateSaleCommand(
    string InvoiceNumber,
    string? LabOrderNumber,
    int PatientId,
    string PatientName,
    int UserId,
    string UserName,
    decimal TotalAmount,
    decimal Adelanto,
    string? DiscountCode,
    decimal DiscountAmount,
    string PaymentMethod,
    string CreatedAt,
    string? DeliveredAt,
    string? Notes);
