using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.Inventory.Interfaces.REST.Resources;

/// <summary>
///     Represents the data provided by the server about a stock audit log entry.
/// </summary>
[SwaggerSchema(Description = "A stock audit log resource")]
public record StockAuditLogResource(
    [SwaggerParameter(Description = "The server-generated ID of the audit log entry")] int Id,
    [SwaggerParameter(Description = "Reference to the affected product")] int ProductId,
    [SwaggerParameter(Description = "Name of the affected product")] string ProductName,
    [SwaggerParameter(Description = "Stock keeping unit code of the affected product")] string Sku,
    [SwaggerParameter(Description = "Operation performed: Restock or Manual Adjustment")] string Operation,
    [SwaggerParameter(Description = "Stock quantity before the operation")] int PreviousStock,
    [SwaggerParameter(Description = "Quantity added (positive) or removed (negative) by the operation")] int Quantity,
    [SwaggerParameter(Description = "Stock quantity after the operation")] int NewStock,
    [SwaggerParameter(Description = "Name of the technical who performed the operation")] string Author,
    [SwaggerParameter(Description = "Date the operation was recorded (YYYY-MM-DD)")] string Date,
    [SwaggerParameter(Description = "Time the operation was recorded (HH:mm:ss)")] string Time);
