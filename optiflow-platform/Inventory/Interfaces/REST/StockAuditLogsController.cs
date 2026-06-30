using System.Net.Mime;
using optiflow_platform.Inventory.Application.Services;
using optiflow_platform.Inventory.Domain.Model.Queries;
using optiflow_platform.Inventory.Interfaces.REST.Resources;
using optiflow_platform.Inventory.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using optiflow_platform.IAM.Infrastructure.Pipeline.Middleware.Attributes;

namespace optiflow_platform.Inventory.Interfaces.REST;

/// <summary>
///     Stock audit logs controller.
/// </summary>
[ApiController]
[Route("/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Stock Audit Logs")]
[Authorize]
public class StockAuditLogsController(IStockAuditLogQueryService stockAuditLogQueryService) : ControllerBase
{
    /// <summary>
    ///     Gets the stock audit log entries, optionally filtered by a recorded date period.
    /// </summary>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Gets stock audit log entries",
        Description = "Returns the stock audit history, optionally filtered to a date period (YYYY-MM-DD) for the Inventory Audit flow",
        OperationId = "GetStockAuditLogs")]
    [SwaggerResponse(200, "List of stock audit log entries", typeof(IEnumerable<StockAuditLogResource>))]
    public async Task<ActionResult> GetStockAuditLogs(
        [FromQuery] [SwaggerParameter(Description = "Start of the date period (YYYY-MM-DD)")] string? from,
        [FromQuery] [SwaggerParameter(Description = "End of the date period (YYYY-MM-DD)")] string? to,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAuditLogsQuery(from, to);
        var result = await stockAuditLogQueryService.Handle(query, cancellationToken);
        var resources = result.Select(StockAuditLogResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }
}
