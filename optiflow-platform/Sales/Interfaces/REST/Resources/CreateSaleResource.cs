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
    [Required] [Range(0.01, 1000000, ErrorMessage = "TotalAmount must be between 0.01 and 1000000")] [SwaggerParameter(Description = "Total sale amount")] decimal TotalAmount,
    [Range(0, 1000000, ErrorMessage = "Advance must be between 0 and 1000000")] [SwaggerParameter(Description = "Advance payment amount")] decimal Advance,
    [SwaggerParameter(Description = "Discount code applied")] string? DiscountCode,
    [Range(0, 1000000, ErrorMessage = "DiscountAmount must be between 0 and 1000000")] [SwaggerParameter(Description = "Fixed discount amount")] decimal DiscountAmount,
    [Required] [SwaggerParameter(Description = "Payment method (e.g. CREDIT_CARD, CASH, DEBIT_CARD, TRANSFER)")] string PaymentMethod,
    [Required] [SwaggerParameter(Description = "Sale creation date (ISO 8601)")] string CreatedAt,
    [SwaggerParameter(Description = "Expected delivery date (ISO 8601)")] string? DeliveredAt,
    [SwaggerParameter(Description = "Additional notes")] string? Notes,
    [Required] [MinLength(1)] [SwaggerParameter(Description = "Products sold as part of this sale (e.g. frame, lens, contact lenses, accessories)")] IReadOnlyList<CreateSaleItemResource> Items);

[SwaggerSchema(Description = "A product and quantity sold as part of a sale")]
public record CreateSaleItemResource(
    [Required] [Range(1, int.MaxValue, ErrorMessage = "ProductId must be a positive identifier")] [SwaggerParameter(Description = "Inventory product ID")] int ProductId,
    [Required] [Range(1, 1000000, ErrorMessage = "Quantity must be between 1 and 1000000")] [SwaggerParameter(Description = "Quantity sold")] int Quantity);
