using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.Sales.Interfaces.REST.Resources;

[SwaggerSchema(Description = "A payment resource")]
public record PaymentResource(
    [SwaggerParameter(Description = "The server-generated ID of the payment")] int Id,
    [SwaggerParameter(Description = "Reference to the related sale")] int SaleId,
    [SwaggerParameter(Description = "Amount paid in this transaction")] decimal AmountPaid,
    [SwaggerParameter(Description = "Payment method used (e.g. CASH, CARD, TRANSFER)")] string Method,
    [SwaggerParameter(Description = "Timestamp when the payment was processed")] string PaidAt);
