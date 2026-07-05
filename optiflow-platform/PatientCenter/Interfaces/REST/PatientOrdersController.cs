using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using optiflow_platform.LabAndOrders.Application.Services;
using optiflow_platform.LabAndOrders.Domain.Model.Queries;
using optiflow_platform.PatientCenter.Interfaces.REST.Resources;
using optiflow_platform.Sales.Application.Services;
using optiflow_platform.Sales.Domain.Model.Queries;
using Swashbuckle.AspNetCore.Annotations;
using optiflow_platform.IAM.Infrastructure.Pipeline.Middleware.Attributes;

namespace optiflow_platform.PatientCenter.Interfaces.REST;

[ApiController]
[Route("api/v1/patients/{patientId}/orders")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Patient Orders")]
[Authorize]
[AllowWithoutAccount]
[AllowWithoutSubscription]
public class PatientOrdersController(
    IWorkOrderQueryService workOrderQueryService,
    ISaleQueryService saleQueryService,
    ILogger<PatientOrdersController> logger)
    : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(
        Summary = "Gets all orders for a patient",
        Description = "Returns work orders with payment info for the given patient",
        OperationId = "GetOrdersByPatientId")]
    [SwaggerResponse(200, "List of orders", typeof(IEnumerable<PatientOrderResource>))]
    [SwaggerResponse(500, "Unexpected error")]
    public async Task<ActionResult> GetOrdersByPatientId(int patientId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var workOrders = await workOrderQueryService.Handle(
                new GetAllWorkOrdersQuery(), cancellationToken);

            var sales = await saleQueryService.Handle(
                new GetAllSalesQuery(), cancellationToken);

            var patientSales = sales
                .Where(s => s.PatientId == patientId)
                .ToDictionary(s => s.Id.Value);

            var patientOrders = workOrders
                .Where(wo => patientSales.ContainsKey(wo.SaleId))
                .Select(wo =>
                {
                    var sale = patientSales[wo.SaleId];
                    return new PatientOrderResource(
                        wo.Id,
                        wo.SaleId,
                        $"LAB-{wo.Id:D4}",
                        wo.PatientName,
                        wo.LensType,
                        wo.Frame,
                        wo.Status,
                        wo.Priority,
                        wo.DeliveryDate,
                        sale.CreatedAt,
                        sale.TotalAmount,
                        sale.Advance,
                        sale.PendingBalance,
                        wo.IsRework);
                })
                .ToList();

            return Ok(patientOrders);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting orders for patient {PatientId}", patientId);
            return Problem(statusCode: 500);
        }
    }
}