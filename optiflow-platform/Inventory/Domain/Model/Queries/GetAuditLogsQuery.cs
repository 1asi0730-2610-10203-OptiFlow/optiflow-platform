namespace optiflow_platform.Inventory.Domain.Model.Queries;

/// <summary>
///     Query for the Inventory Audit History View read model, optionally filtered by a date period (YYYY-MM-DD).
/// </summary>
/// <remarks>
///     Backs the "Run Inventory Audit" flow: when both bounds are provided, only audit log
///     entries recorded within the period are returned.
/// </remarks>
public record GetAuditLogsQuery(string? From, string? To);
