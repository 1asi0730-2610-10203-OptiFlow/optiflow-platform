using optiflow_platform.Inventory.Domain.Model.Aggregates;
using optiflow_platform.Inventory.Interfaces.REST.Resources;

namespace optiflow_platform.Inventory.Interfaces.REST.Transform;

/// <summary>
///     Assembles a StockAuditLogResource from a StockAuditLog aggregate.
/// </summary>
public static class StockAuditLogResourceFromEntityAssembler
{
    /// <summary>
    ///     Converts a StockAuditLog entity to a StockAuditLogResource.
    /// </summary>
    public static StockAuditLogResource ToResourceFromEntity(StockAuditLog auditLog) =>
        new(auditLog.Id, auditLog.ProductId, auditLog.ProductName, auditLog.Sku, auditLog.Operation,
            auditLog.PreviousStock, auditLog.Quantity, auditLog.NewStock, auditLog.Author,
            auditLog.Date, auditLog.Time);
}
