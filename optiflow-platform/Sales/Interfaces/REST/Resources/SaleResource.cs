using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.Sales.Interfaces.REST.Resources;

[SwaggerSchema(Description = "A sale resource")]
public record SaleResource(
    [SwaggerParameter(Description = "Server-generated sale ID")] int Id,
    [SwaggerParameter(Description = "Invoice number")] string InvoiceNumber,
    [SwaggerParameter(Description = "Associated lab order number")] string LabOrderNumber,
    [SwaggerParameter(Description = "Patient ID")] int PatientId,
    [SwaggerParameter(Description = "Full name of the patient")] string PatientName,
    [SwaggerParameter(Description = "ID of the staff member who created the sale")] int UserId,
    [SwaggerParameter(Description = "Name of the staff member who created the sale")] string UserName,
    [SwaggerParameter(Description = "Total sale amount after discount")] decimal TotalAmount,
    [SwaggerParameter(Description = "Advance payment amount")] decimal Advance,
    [SwaggerParameter(Description = "Remaining balance to pay")] decimal PendingBalance,
    [SwaggerParameter(Description = "Discount code applied")] string DiscountCode,
    [SwaggerParameter(Description = "Fixed discount amount applied")] decimal DiscountAmount,
    [SwaggerParameter(Description = "Current status of the sale")] string Status,
    [SwaggerParameter(Description = "Payment method")] string PaymentMethod,
    [SwaggerParameter(Description = "Sale creation date")] string CreatedAt,
    [SwaggerParameter(Description = "Delivery date")] string DeliveredAt,
    [SwaggerParameter(Description = "Additional notes")] string Notes,
    [SwaggerParameter(Description = "Products sold as part of this sale")] IEnumerable<SaleItemResource> Items);
