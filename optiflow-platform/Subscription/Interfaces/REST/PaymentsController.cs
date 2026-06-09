using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using optiflow_platform.Shared.Application.Patterns;
using optiflow_platform.Subscription.Application.Errors;
using optiflow_platform.Subscription.Application.Services;
using optiflow_platform.Subscription.Domain.Model.Aggregates;
using optiflow_platform.Subscription.Domain.Model.Queries;
using optiflow_platform.Subscription.Domain.Model.ValueObjects;
using optiflow_platform.Subscription.Interfaces.REST.Resources;
using optiflow_platform.Subscription.Interfaces.REST.Transform;
using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.Subscription.Interfaces.REST;

[ApiController]
[Route("api/v1/subscription-payments")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Subscription Payments")]
public class SubscriptionPaymentsController(
    IPaymentCommandService paymentCommandService,
    IPaymentQueryService paymentQueryService,
    ILogger<SubscriptionPaymentsController> logger)
    : ControllerBase
{
    /// <summary>Processes a payment for a subscription.</summary>
    [HttpPost("subscriptions/{subscriptionId:int}")]
    [SwaggerOperation(Summary = "Processes a subscription payment", OperationId = "ProcessSubscriptionPayment")]
    [SwaggerResponse(201, "Payment processed", typeof(SubscriptionPaymentResource))]
    [SwaggerResponse(400, "Invalid request payload", typeof(string))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> ProcessPayment(int subscriptionId,
        [FromBody] ProcessPaymentResource resource, CancellationToken cancellationToken)
    {
        try
        {
            var command = ProcessPaymentCommandFromResourceAssembler.ToCommandFromResource(subscriptionId, resource);
            var result = await paymentCommandService.Handle(command, cancellationToken);
            return result switch
            {
                Result<Payment, ProcessSubscriptionPaymentError>.Success success =>
                    CreatedAtAction(nameof(GetSubscriptionPaymentById),
                        new { id = success.Value.Id },
                        SubscriptionPaymentResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
                _ => Problem(title: "Unexpected error", detail: "Could not process payment.", statusCode: 500)
            };
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Validation failed processing payment for subscription {Id}", subscriptionId);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error processing payment for subscription {Id}", subscriptionId);
            return Problem(title: "Unexpected server error", detail: "Could not process payment.", statusCode: 500);
        }
    }

    /// <summary>Gets a subscription payment by id.</summary>
    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Gets a subscription payment by id", OperationId = "GetSubscriptionPaymentById")]
    [SwaggerResponse(200, "The payment was found", typeof(SubscriptionPaymentResource))]
    [SwaggerResponse(404, "Payment not found")]
    public async Task<ActionResult> GetSubscriptionPaymentById(int id, CancellationToken cancellationToken = default)
    {
        var result = await paymentQueryService.Handle(new GetPaymentByIdQuery(new PaymentId(id)), cancellationToken);
        if (result is null) return NotFound();
        return Ok(SubscriptionPaymentResourceFromEntityAssembler.ToResourceFromEntity(result));
    }

    /// <summary>Gets all payments for a subscription.</summary>
    [HttpGet("subscriptions/{subscriptionId:int}")]
    [SwaggerOperation(Summary = "Gets subscription payments by subscription", OperationId = "GetSubscriptionPaymentsBySubscriptionId")]
    [SwaggerResponse(200, "List of payments for the subscription", typeof(IEnumerable<SubscriptionPaymentResource>))]
    public async Task<ActionResult> GetSubscriptionPaymentsBySubscriptionId(int subscriptionId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetPaymentsBySubscriptionIdQuery(new SubscriptionId(subscriptionId));
        var result = await paymentQueryService.Handle(query, cancellationToken);
        return Ok(result.Select(SubscriptionPaymentResourceFromEntityAssembler.ToResourceFromEntity));
    }
}
