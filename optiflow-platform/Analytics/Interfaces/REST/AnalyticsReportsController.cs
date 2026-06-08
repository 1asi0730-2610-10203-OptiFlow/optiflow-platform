using System.Net.Mime;
using optiflow_platform.Analytics.Application.Services;
using optiflow_platform.Analytics.Domain.Model.Queries;
using optiflow_platform.Analytics.Interfaces.REST.Resources;
using optiflow_platform.Analytics.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.Analytics.Interfaces.REST;

/// <summary>
///     Analytics reports controller.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Analytics Reports")]
public class AnalyticsReportsController(
    IAnalyticsReportQueryService analyticsReportQueryService,
    ILogger<AnalyticsReportsController> logger)
    : ControllerBase
{
    /// <summary>
    ///     Gets all analytics reports.
    /// </summary>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Gets all analytics reports",
        Description = "Returns all analytics reports stored in the system",
        OperationId = "GetAllAnalyticsReports")]
    [SwaggerResponse(200, "List of analytics reports", typeof(IEnumerable<AnalyticsReportResource>))]
    public async Task<ActionResult> GetAllAnalyticsReports(CancellationToken cancellationToken = default)
    {
        var query = new GetAllAnalyticsReportsQuery();
        var result = await analyticsReportQueryService.Handle(query, cancellationToken);
        var resources = result.Select(AnalyticsReportResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    /// <summary>
    ///     Gets an analytics report by id.
    /// </summary>
    [HttpGet("{id:int}")]
    [SwaggerOperation(
        Summary = "Gets an analytics report by id",
        Description = "Returns a single analytics report for the given identifier",
        OperationId = "GetAnalyticsReportById")]
    [SwaggerResponse(200, "The analytics report was found", typeof(AnalyticsReportResource))]
    [SwaggerResponse(404, "The analytics report was not found")]
    public async Task<ActionResult> GetAnalyticsReportById(int id, CancellationToken cancellationToken = default)
    {
        var query = new GetAnalyticsReportByIdQuery(id);
        var result = await analyticsReportQueryService.Handle(query, cancellationToken);
        if (result is null) return NotFound();
        return Ok(AnalyticsReportResourceFromEntityAssembler.ToResourceFromEntity(result));
    }

    /// <summary>
    ///     Gets analytics reports filtered by period.
    /// </summary>
    [HttpGet("by-period/{period}")]
    [SwaggerOperation(
        Summary = "Gets analytics reports by period",
        Description = "Returns all analytics reports for the given period (e.g. '2026-05')",
        OperationId = "GetAnalyticsReportsByPeriod")]
    [SwaggerResponse(200, "List of analytics reports for the period", typeof(IEnumerable<AnalyticsReportResource>))]
    public async Task<ActionResult> GetAnalyticsReportsByPeriod(string period, CancellationToken cancellationToken = default)
    {
        var query = new GetAnalyticsReportsByPeriodQuery(period);
        var result = await analyticsReportQueryService.Handle(query, cancellationToken);
        var resources = result.Select(AnalyticsReportResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }
}
