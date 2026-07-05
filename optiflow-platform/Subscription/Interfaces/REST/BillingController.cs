using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using optiflow_platform.Shared.Application.Patterns;
using optiflow_platform.Subscription.Application.Errors;
using optiflow_platform.Subscription.Application.Services;
using optiflow_platform.Subscription.Domain.Model.Aggregates;
using optiflow_platform.Subscription.Domain.Model.Commands;
using optiflow_platform.Subscription.Domain.Model.Queries;
using optiflow_platform.Subscription.Domain.Model.ValueObjects;
using optiflow_platform.Subscription.Interfaces.REST.Resources;
using optiflow_platform.Subscription.Interfaces.REST.Transform;
using Swashbuckle.AspNetCore.Annotations;
using optiflow_platform.IAM.Infrastructure.Pipeline.Middleware.Attributes;

namespace optiflow_platform.Subscription.Interfaces.REST;

[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Billing")]
[Authorize]
[AllowWithoutSubscription]
public class BillingController(
    IBillingCommandService billingCommandService,
    IBillingQueryService billingQueryService,
    ILogger<BillingController> logger)
    : ControllerBase
{
    /// <summary>Checks whether a subscription is due for renewal and creates a billing record.</summary>
    [HttpPost("subscriptions/{subscriptionId:int}/check-renewal")]
    [SwaggerOperation(Summary = "Checks subscription renewal", OperationId = "CheckSubscriptionRenewal")]
    [SwaggerResponse(201, "Billing record created", typeof(BillingResource))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> CheckRenewal(int subscriptionId, CancellationToken cancellationToken)
    {
        try
        {
            var command = new CheckSubscriptionRenewalCommand(new SubscriptionId(subscriptionId));
            var result = await billingCommandService.Handle(command, cancellationToken);
            return result switch
            {
                Result<Billing, RenewSubscriptionError>.Success success =>
                    CreatedAtAction(nameof(GetBillingBySubscriptionId),
                        new { subscriptionId },
                        BillingResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
                _ => Problem(title: "Unexpected error", detail: "Could not check renewal.", statusCode: 500)
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error checking renewal for subscription {Id}", subscriptionId);
            return Problem(title: "Unexpected server error", detail: "Could not check renewal.", statusCode: 500);
        }
    }

    /// <summary>Requests an automatic renewal charge for a subscription.</summary>
    [HttpPost("subscriptions/{subscriptionId:int}/auto-renew")]
    [SwaggerOperation(Summary = "Requests auto-renewal", OperationId = "RequestAutoRenew")]
    [SwaggerResponse(200, "Auto-renewal processed", typeof(BillingResource))]
    [SwaggerResponse(404, "Billing record not found")]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> RequestAutoRenew(int subscriptionId, CancellationToken cancellationToken)
    {
        try
        {
            var command = new RequestAutoRenewCommand(new SubscriptionId(subscriptionId));
            var result = await billingCommandService.Handle(command, cancellationToken);
            return result switch
            {
                Result<Billing, RenewSubscriptionError>.Success success =>
                    Ok(BillingResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
                Result<Billing, RenewSubscriptionError>.Failure { Error: RenewSubscriptionError.BillingNotFound } =>
                    NotFound(),
                _ => Problem(title: "Unexpected error", detail: "Could not process auto-renewal.", statusCode: 500)
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error auto-renewing subscription {Id}", subscriptionId);
            return Problem(title: "Unexpected server error", detail: "Could not process auto-renewal.", statusCode: 500);
        }
    }

    /// <summary>Gets the billing record for a subscription.</summary>
    [HttpGet("subscriptions/{subscriptionId:int}")]
    [SwaggerOperation(Summary = "Gets billing by subscription", OperationId = "GetBillingBySubscriptionId")]
    [SwaggerResponse(200, "The billing record was found", typeof(BillingResource))]
    [SwaggerResponse(404, "Billing record not found")]
    public async Task<ActionResult> GetBillingBySubscriptionId(int subscriptionId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetBillingBySubscriptionIdQuery(new SubscriptionId(subscriptionId));
        var result = await billingQueryService.Handle(query, cancellationToken);
        if (result is null) return NotFound();
        return Ok(BillingResourceFromEntityAssembler.ToResourceFromEntity(result));
    }
}
