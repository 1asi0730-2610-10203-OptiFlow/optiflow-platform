using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using optiflow_platform.Clinical.Domain.Model.Aggregates;
using optiflow_platform.LabAndOrders.Application.Services;
using optiflow_platform.LabAndOrders.Domain.Model.Queries;
using optiflow_platform.PatientCenter.Interfaces.REST.Resources;
using optiflow_platform.Sales.Application.Services;
using optiflow_platform.Sales.Domain.Model.Queries;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Configuration;
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
    AppDbContext dbContext,
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
            // The patient portal is cross-account by design (clients aren't the optic's tenant). Scope the
            // order lookups to the OPTIC that owns this patient — resolved ignoring the tenant filter — instead
            // of the client's own session account, otherwise the account query filter hides every order.
            var opticAccountId = await dbContext.Set<Patient>()
                .IgnoreQueryFilters()
                .Where(p => p.Id == patientId)
                .Select(p => (Guid?)p.AccountId)
                .FirstOrDefaultAsync(cancellationToken);
            if (opticAccountId is { } accountId)
                dbContext.CurrentAccountId = accountId;

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