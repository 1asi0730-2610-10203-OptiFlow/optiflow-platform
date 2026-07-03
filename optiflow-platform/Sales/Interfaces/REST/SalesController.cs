using System.Net.Mime;
using optiflow_platform.Sales.Application.Errors;
using optiflow_platform.Sales.Application.Services;
using optiflow_platform.Sales.Domain.Model.Aggregates;
using optiflow_platform.Sales.Domain.Model.Commands;
using optiflow_platform.Sales.Domain.Model.Queries;
using optiflow_platform.Sales.Domain.Model.ValueObjects;
using optiflow_platform.Sales.Interfaces.REST.Resources;
using optiflow_platform.Sales.Interfaces.REST.Transform;
using optiflow_platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using optiflow_platform.IAM.Infrastructure.Pipeline.Middleware.Attributes;

namespace optiflow_platform.Sales.Interfaces.REST;

/// <summary>
///     Sales controller.
/// </summary>
[ApiController]
[Route("/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Sales")]
[Authorize]
public class SalesController(
    ISaleCommandService saleCommandService,
    ISaleQueryService saleQueryService,
    ILogger<SalesController> logger)
    : ControllerBase
{
    /// <summary>
    ///     Creates a new sale.
    /// </summary>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Creates a sale",
        Description = "Creates a new sale record in ACTIVE status",
        OperationId = "CreateSale")]
    [SwaggerResponse(201, "The sale was created", typeof(SaleResource))]
    [SwaggerResponse(400, "The request payload is invalid", typeof(string))]
    [SwaggerResponse(409, "A sale with this invoice number already exists")]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> CreateSale([FromBody] CreateSaleResource resource,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = CreateSaleCommandFromResourceAssembler.ToCommandFromResource(resource);
            var result = await saleCommandService.Handle(command, cancellationToken);
            return result switch
            {
                Result<Sale, CreateSaleError>.Success success =>
                    CreatedAtAction(nameof(GetSaleById),
                        new { id = success.Value.Id },
                        SaleResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
                Result<Sale, CreateSaleError>.Failure { Error: CreateSaleError.DuplicateInvoiceNumber } =>
                    Conflict("A sale with this invoice number already exists."),
                Result<Sale, CreateSaleError>.Failure =>
                    Problem(title: "Unexpected error", detail: "Could not create sale.", statusCode: 500),
                _ => Problem(statusCode: 500)
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error creating sale for patient {PatientName}", resource.PatientName);
            return Problem(title: "Unexpected server error",
                detail: "An unexpected error occurred while creating the sale.", statusCode: 500);
        }
    }

    /// <summary>
    ///     Gets all sales.
    /// </summary>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Gets all sales",
        Description = "Returns all sales in the system",
        OperationId = "GetAllSales")]
    [SwaggerResponse(200, "List of sales", typeof(IEnumerable<SaleResource>))]
    public async Task<ActionResult> GetAllSales(CancellationToken cancellationToken = default)
    {
        var query = new GetAllSalesQuery();
        var result = await saleQueryService.Handle(query, cancellationToken);
        var resources = result.Select(SaleResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    /// <summary>
    ///     Gets a sale by id.
    /// </summary>
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Gets a sale by id",
        Description = "Returns a single sale for the given identifier",
        OperationId = "GetSaleById")]
    [SwaggerResponse(200, "The sale was found", typeof(SaleResource))]
    [SwaggerResponse(404, "The sale was not found")]
    public async Task<ActionResult> GetSaleById(int id, CancellationToken cancellationToken = default)
    {
        var query = new GetSaleByIdQuery(new SaleId(id));
        var result = await saleQueryService.Handle(query, cancellationToken);
        if (result is null) return NotFound();
        return Ok(SaleResourceFromEntityAssembler.ToResourceFromEntity(result));
    }

    /// <summary>
    ///     Generates a sale quota.
    /// </summary>
    [HttpPost("{id}/generate-quota")]
    [SwaggerOperation(
        Summary = "Generates a sale quota",
        Description = "Sets the advance quota (must be at least 30% of the total sale amount)",
        OperationId = "GenerateSaleQuota")]
    [SwaggerResponse(200, "The quota was generated", typeof(SaleResource))]
    [SwaggerResponse(400, "Quota is below the 30% minimum", typeof(string))]
    [SwaggerResponse(404, "The sale was not found")]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> GenerateSaleQuota(int id, [FromBody] GenerateSaleQuotaResource resource,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = GenerateSaleQuotaCommandFromResourceAssembler.ToCommandFromResource(new SaleId(id), resource);
            var result = await saleCommandService.Handle(command, cancellationToken);
            return result switch
            {
                Result<Sale, GenerateSaleQuotaError>.Success success =>
                    Ok(SaleResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
                Result<Sale, GenerateSaleQuotaError>.Failure { Error: GenerateSaleQuotaError.SaleNotFound } =>
                    NotFound(),
                Result<Sale, GenerateSaleQuotaError>.Failure { Error: GenerateSaleQuotaError.QuotaBelowMinimum } =>
                    BadRequest("Quota amount must be at least 30% of the total sale amount."),
                _ => Problem(title: "Unexpected server error", detail: "Could not generate sale quota.", statusCode: 500)
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error generating quota for sale {SaleId}", id);
            return Problem(title: "Unexpected server error",
                detail: "An unexpected error occurred while generating the sale quota.", statusCode: 500);
        }
    }

    /// <summary>
    ///     Requests cancellation of a sale.
    /// </summary>
    [HttpPost("{id}/request-cancellation")]
    [SwaggerOperation(
        Summary = "Requests sale cancellation",
        Description = "Marks the sale as pending cancellation review",
        OperationId = "RequestSaleCancellation")]
    [SwaggerResponse(200, "Cancellation was requested", typeof(SaleResource))]
    [SwaggerResponse(404, "The sale was not found")]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> RequestSaleCancellation(int id, CancellationToken cancellationToken)
    {
        try
        {
            var command = new RequestSaleCancellationCommand(new SaleId(id));
            var result = await saleCommandService.Handle(command, cancellationToken);
            return result switch
            {
                Result<Sale, RequestSaleCancellationError>.Success success =>
                    Ok(SaleResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
                Result<Sale, RequestSaleCancellationError>.Failure
                    { Error: RequestSaleCancellationError.SaleNotFound } =>
                    NotFound(),
                _ => Problem(title: "Unexpected server error",
                    detail: "Could not request sale cancellation.", statusCode: 500)
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error requesting cancellation for sale {SaleId}", id);
            return Problem(title: "Unexpected server error",
                detail: "An unexpected error occurred while requesting sale cancellation.", statusCode: 500);
        }
    }

    /// <summary>
    ///     Cancels a sale.
    /// </summary>
    [HttpPost("{id}/cancel")]
    [SwaggerOperation(
        Summary = "Cancels a sale",
        Description = "Cancels the sale. Returns 409 if the lab order is currently in production.",
        OperationId = "CancelSale")]
    [SwaggerResponse(200, "The sale was cancelled", typeof(SaleResource))]
    [SwaggerResponse(404, "The sale was not found")]
    [SwaggerResponse(409, "Cannot cancel: lab order is in production")]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> CancelSale(int id, [FromBody] CancelSaleResource resource,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = CancelSaleCommandFromResourceAssembler.ToCommandFromResource(new SaleId(id), resource);
            var result = await saleCommandService.Handle(command, cancellationToken);
            return result switch
            {
                Result<Sale, CancelSaleError>.Success success =>
                    Ok(SaleResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
                Result<Sale, CancelSaleError>.Failure { Error: CancelSaleError.SaleNotFound } =>
                    NotFound(),
                Result<Sale, CancelSaleError>.Failure { Error: CancelSaleError.SaleInProductionStatus } =>
                    Conflict("Cannot cancel sale: lab order is currently in production."),
                _ => Problem(title: "Unexpected server error", detail: "Could not cancel sale.", statusCode: 500)
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error cancelling sale {SaleId}", id);
            return Problem(title: "Unexpected server error",
                detail: "An unexpected error occurred while cancelling the sale.", statusCode: 500);
        }
    }

    /// <summary>
    ///     Applies a promotional discount to a sale.
    /// </summary>
    [HttpPost("{id}/apply-discount")]
    [SwaggerOperation(
        Summary = "Applies a promotional discount",
        Description = "Applies a percentage discount to the sale total",
        OperationId = "ApplyPromotionalDiscount")]
    [SwaggerResponse(200, "The discount was applied", typeof(SaleResource))]
    [SwaggerResponse(404, "The sale was not found")]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> ApplyPromotionalDiscount(int id,
        [FromBody] ApplyPromotionalDiscountResource resource, CancellationToken cancellationToken)
    {
        try
        {
            var command = ApplyPromotionalDiscountCommandFromResourceAssembler.ToCommandFromResource(new SaleId(id), resource);
            var result = await saleCommandService.Handle(command, cancellationToken);
            return result switch
            {
                Result<Sale, ApplyPromotionalDiscountError>.Success success =>
                    Ok(SaleResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
                Result<Sale, ApplyPromotionalDiscountError>.Failure
                    { Error: ApplyPromotionalDiscountError.SaleNotFound } =>
                    NotFound(),
                _ => Problem(title: "Unexpected server error", detail: "Could not apply discount.", statusCode: 500)
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error applying discount to sale {SaleId}", id);
            return Problem(title: "Unexpected server error",
                detail: "An unexpected error occurred while applying the discount.", statusCode: 500);
        }
    }
}
