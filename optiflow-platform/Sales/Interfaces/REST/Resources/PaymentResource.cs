using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.Sales.Interfaces.REST.Resources;

[SwaggerSchema(Description = "A payment resource")]
public record PaymentResource(
    [SwaggerParameter(Description = "The server-generated ID of the payment")] int Id,
    [SwaggerParameter(Description = "Reference to the related sale")] int SaleId,
    [SwaggerParameter(Description = "Total amount to be paid")] decimal TotalAmount,
    [SwaggerParameter(Description = "Amount already paid")] decimal PaidAmount,
    [SwaggerParameter(Description = "Remaining outstanding balance")] decimal OutstandingBalance,
    [SwaggerParameter(Description = "Current status of the payment")] string Status);
