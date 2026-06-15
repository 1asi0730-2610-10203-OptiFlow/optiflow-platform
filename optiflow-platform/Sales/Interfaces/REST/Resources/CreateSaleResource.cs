using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.Sales.Interfaces.REST.Resources;

[SwaggerSchema(Description = "Payload to create a new sale")]
public record CreateSaleResource(
    [Required] [SwaggerParameter(Description = "Invoice number (e.g. FAC-2843)")] string InvoiceNumber,
    [SwaggerParameter(Description = "Associated lab order number")] string? LabOrderNumber,
    [Required] [SwaggerParameter(Description = "Patient ID")] int PatientId,
    [Required] [SwaggerParameter(Description = "Full name of the patient")] string PatientName,
    [Required] [SwaggerParameter(Description = "ID of the staff member creating the sale")] int UserId,
    [Required] [SwaggerParameter(Description = "Name of the staff member creating the sale")] string UserName,
    [Required] [SwaggerParameter(Description = "Total sale amount")] decimal TotalAmount,
    [SwaggerParameter(Description = "Advance payment amount")] decimal Advance,
    [SwaggerParameter(Description = "Discount code applied")] string? DiscountCode,
    [SwaggerParameter(Description = "Fixed discount amount")] decimal DiscountAmount,
    [Required] [SwaggerParameter(Description = "Payment method (e.g. CREDIT_CARD, CASH, DEBIT_CARD, TRANSFER)")] string PaymentMethod,
    [Required] [SwaggerParameter(Description = "Sale creation date (ISO 8601)")] string CreatedAt,
    [SwaggerParameter(Description = "Expected delivery date (ISO 8601)")] string? DeliveredAt,
    [SwaggerParameter(Description = "Additional notes")] string? Notes);
