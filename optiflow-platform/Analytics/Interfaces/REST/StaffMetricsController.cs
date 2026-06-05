using System.Net.Mime;
using optiflow_platform.Analytics.Application.Services;
using optiflow_platform.Analytics.Domain.Model.Queries;
using optiflow_platform.Analytics.Interfaces.REST.Resources;
using optiflow_platform.Analytics.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.Analytics.Interfaces.REST;

/// <summary>
///     Staff metrics controller.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Staff Metrics")]
public class StaffMetricsController(
    IStaffMetricQueryService staffMetricQueryService,
    ILogger<StaffMetricsController> logger)
    : ControllerBase
{
    /// <summary>
    ///     Gets all staff metrics.
    /// </summary>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Gets all staff metrics",
        Description = "Returns all staff metrics across all analytics reports",
        OperationId = "GetAllStaffMetrics")]
    [SwaggerResponse(200, "List of staff metrics", typeof(IEnumerable<StaffMetricResource>))]
    public async Task<ActionResult> GetAllStaffMetrics(CancellationToken cancellationToken = default)
    {
        var query = new GetAllStaffMetricsQuery();
        var result = await staffMetricQueryService.Handle(query, cancellationToken);
        var resources = result.Select(StaffMetricResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    /// <summary>
    ///     Gets staff metrics for a specific analytics report.
    /// </summary>
    [HttpGet("by-report/{reportId:int}")]
    [SwaggerOperation(
        Summary = "Gets staff metrics by report id",
        Description = "Returns all staff metrics that belong to the given analytics report",
        OperationId = "GetStaffMetricsByReportId")]
    [SwaggerResponse(200, "List of staff metrics for the report", typeof(IEnumerable<StaffMetricResource>))]
    public async Task<ActionResult> GetStaffMetricsByReportId(int reportId, CancellationToken cancellationToken = default)
    {
        var query = new GetStaffMetricsByReportIdQuery(reportId);
        var result = await staffMetricQueryService.Handle(query, cancellationToken);
        var resources = result.Select(StaffMetricResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }
}
