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
///     Payments controller.
/// </summary>
[ApiController]
[Route("/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Payments")]
[Authorize]
public class PaymentsController(
    IPaymentCommandService paymentCommandService,
    IPaymentQueryService paymentQueryService,
    ISaleCommandService saleCommandService,
    ILogger<PaymentsController> logger)
    : ControllerBase
{
    /// <summary>
    ///     Gets all payments.
    /// </summary>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Gets all payments",
        Description = "Returns all payment records in the system",
        OperationId = "GetAllPayments")]
    [SwaggerResponse(200, "List of payments", typeof(IEnumerable<PaymentResource>))]
    public async Task<ActionResult> GetAllPayments(CancellationToken cancellationToken = default)
    {
        var query = new GetAllPaymentsQuery();
        var result = await paymentQueryService.Handle(query, cancellationToken);
        var resources = result.Select(PaymentResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    /// <summary>
    ///     Gets a payment by id.
    /// </summary>
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Gets a payment by id",
        Description = "Returns a single payment for the given identifier",
        OperationId = "GetPaymentById")]
    [SwaggerResponse(200, "The payment was found", typeof(PaymentResource))]
    [SwaggerResponse(404, "The payment was not found")]
    public async Task<ActionResult> GetPaymentById(int id, CancellationToken cancellationToken = default)
    {
        var query = new GetPaymentByIdQuery(new PaymentId(id));
        var result = await paymentQueryService.Handle(query, cancellationToken);
        if (result is null) return NotFound();
        return Ok(PaymentResourceFromEntityAssembler.ToResourceFromEntity(result));
    }

    /// <summary>
    ///     Gets the payment associated with a sale.
    /// </summary>
    [HttpGet("by-sale/{saleId}")]
    [SwaggerOperation(
        Summary = "Gets a payment by sale id",
        Description = "Returns the payment record associated with the given sale",
        OperationId = "GetPaymentBySaleId")]
    [SwaggerResponse(200, "The payment was found", typeof(PaymentResource))]
    [SwaggerResponse(404, "No payment found for this sale")]
    public async Task<ActionResult> GetPaymentBySaleId(int saleId, CancellationToken cancellationToken = default)
    {
        var query = new GetPaymentBySaleIdQuery(saleId);
        var result = await paymentQueryService.Handle(query, cancellationToken);
        if (result is null) return NotFound();
        return Ok(PaymentResourceFromEntityAssembler.ToResourceFromEntity(result));
    }

    /// <summary>
    ///     Pays the outstanding balance of a sale.
    /// </summary>
    /// <remarks>
    ///     When the balance is fully cleared, the sale is automatically marked as COMPLETED.
    /// </remarks>
    [HttpPost("{saleId}/pay")]
    [SwaggerOperation(
        Summary = "Pays outstanding balance",
        Description = "Applies a payment to the outstanding balance. Marks the sale as COMPLETED when fully cleared.",
        OperationId = "PayOutstandingBalance")]
    [SwaggerResponse(200, "Payment was processed", typeof(PaymentResource))]
    [SwaggerResponse(400, "The request payload is invalid", typeof(string))]
    [SwaggerResponse(404, "The sale was not found")]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> PayOutstandingBalance(int saleId,
        [FromBody] PayOutstandingBalanceResource resource, CancellationToken cancellationToken)
    {
        try
        {
            var command = PayOutstandingBalanceCommandFromResourceAssembler.ToCommandFromResource(new SaleId(saleId), resource);
            var result = await paymentCommandService.Handle(command, cancellationToken);

            if (result is Result<Payment, PayOutstandingBalanceError>.Failure { Error: PayOutstandingBalanceError.SaleNotFound })
                return NotFound();

            if (result is Result<Payment, PayOutstandingBalanceError>.Failure)
                return Problem(title: "Unexpected server error", detail: "Could not process payment.", statusCode: 500);

            if (result is not Result<Payment, PayOutstandingBalanceError>.Success success)
                return Problem(statusCode: 500);

            var payment = success.Value;

            // Policy: Payment finished successfully → Complete Sale
            if (payment.Status == PaymentStatus.Completed)
            {
                var completeResult = await saleCommandService.Handle(new CompleteSaleCommand(new SaleId(saleId)), cancellationToken);
                if (completeResult is Result<Sale, CompleteSaleError>.Failure)
                    logger.LogWarning("Payment completed for sale {SaleId} but could not mark sale as completed", saleId);
            }

            return Ok(PaymentResourceFromEntityAssembler.ToResourceFromEntity(payment));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error processing payment for sale {SaleId}", saleId);
            return Problem(title: "Unexpected server error",
                detail: "An unexpected error occurred while processing the payment.", statusCode: 500);
        }
    }
}
