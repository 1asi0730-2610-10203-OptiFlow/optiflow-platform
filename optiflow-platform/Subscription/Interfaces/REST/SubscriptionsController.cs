using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using optiflow_platform.Shared.Application.Patterns;
using optiflow_platform.Subscription.Application.Errors;
using optiflow_platform.Subscription.Application.Services;
using optiflow_platform.Subscription.Domain.Model.Commands;
using optiflow_platform.Subscription.Domain.Model.Queries;
using optiflow_platform.Subscription.Domain.Model.ValueObjects;
using optiflow_platform.Subscription.Interfaces.REST.Resources;
using optiflow_platform.Subscription.Interfaces.REST.Transform;
using Swashbuckle.AspNetCore.Annotations;
using SubscriptionAggregate = optiflow_platform.Subscription.Domain.Model.Aggregates.Subscription;
using optiflow_platform.IAM.Infrastructure.Pipeline.Middleware.Attributes;

namespace optiflow_platform.Subscription.Interfaces.REST;

[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Subscriptions")]
[Authorize]
[AllowWithoutSubscription]
public class SubscriptionsController(
    ISubscriptionCommandService subscriptionCommandService,
    ISubscriptionQueryService subscriptionQueryService,
    ILogger<SubscriptionsController> logger)
    : ControllerBase
{
    /// <summary>Creates a new subscription for an admin by selecting a plan.</summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Creates a subscription", OperationId = "SelectSubscriptionPlan")]
    [SwaggerResponse(201, "Subscription created", typeof(SubscriptionResource))]
    [SwaggerResponse(400, "Invalid request payload", typeof(string))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> SelectSubscriptionPlan(
        [FromBody] SelectSubscriptionPlanResource resource, CancellationToken cancellationToken)
    {
        try
        {
            var command = SelectSubscriptionPlanCommandFromResourceAssembler.ToCommandFromResource(resource);
            var result = await subscriptionCommandService.Handle(command, cancellationToken);
            return result switch
            {
                Result<SubscriptionAggregate, SelectSubscriptionPlanError>.Success success =>
                    CreatedAtAction(nameof(GetSubscriptionById),
                        new { id = success.Value.Id },
                        SubscriptionResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
                _ => Problem(title: "Unexpected error", detail: "Could not create subscription.", statusCode: 500)
            };
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Validation failed creating subscription for admin {AdminId}", resource.AdminId);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error creating subscription for admin {AdminId}", resource.AdminId);
            return Problem(title: "Unexpected server error", detail: "Could not create subscription.", statusCode: 500);
        }
    }

    /// <summary>Gets the current account's subscription status.</summary>
    [HttpGet("me")]
    [SwaggerOperation(Summary = "Gets my subscription status", OperationId = "GetMySubscription")]
    [SwaggerResponse(200, "Current subscription status")]
    public async Task<ActionResult> GetMySubscription(CancellationToken cancellationToken = default)
    {
        var active = await subscriptionQueryService.GetCurrentActiveSubscriptionAsync(cancellationToken);
        return Ok(new
        {
            hasActiveSubscription = active != null,
            status = active?.Status.Value,
            subscription = active != null ? SubscriptionResourceFromEntityAssembler.ToResourceFromEntity(active) : null
        });
    }

    /// <summary>Gets all subscriptions.</summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Gets all subscriptions", OperationId = "GetAllSubscriptions")]
    [SwaggerResponse(200, "List of subscriptions", typeof(IEnumerable<SubscriptionResource>))]
    public async Task<ActionResult> GetAllSubscriptions(CancellationToken cancellationToken = default)
    {
        var result = await subscriptionQueryService.Handle(new GetAllSubscriptionsQuery(), cancellationToken);
        return Ok(result.Select(SubscriptionResourceFromEntityAssembler.ToResourceFromEntity));
    }

    /// <summary>Gets a subscription by id.</summary>
    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Gets a subscription by id", OperationId = "GetSubscriptionById")]
    [SwaggerResponse(200, "The subscription was found", typeof(SubscriptionResource))]
    [SwaggerResponse(404, "Subscription not found")]
    public async Task<ActionResult> GetSubscriptionById(int id, CancellationToken cancellationToken = default)
    {
        var result = await subscriptionQueryService.Handle(new GetSubscriptionByIdQuery(new SubscriptionId(id)), cancellationToken);
        if (result is null) return NotFound();
        return Ok(SubscriptionResourceFromEntityAssembler.ToResourceFromEntity(result));
    }

    /// <summary>Gets all subscriptions belonging to an admin.</summary>
    [HttpGet("admin/{adminId:int}")]
    [SwaggerOperation(Summary = "Gets subscriptions by admin", OperationId = "GetSubscriptionsByAdminId")]
    [SwaggerResponse(200, "List of subscriptions for the admin", typeof(IEnumerable<SubscriptionResource>))]
    public async Task<ActionResult> GetSubscriptionsByAdminId(int adminId, CancellationToken cancellationToken = default)
    {
        var result = await subscriptionQueryService.Handle(new GetSubscriptionsByAdminIdQuery(adminId), cancellationToken);
        return Ok(result.Select(SubscriptionResourceFromEntityAssembler.ToResourceFromEntity));
    }

    /// <summary>Activates a subscription after payment is confirmed.</summary>
    [HttpPost("{id:int}/activate")]
    [SwaggerOperation(Summary = "Activates a subscription", OperationId = "ActivateSubscription")]
    [SwaggerResponse(200, "Subscription activated", typeof(SubscriptionResource))]
    [SwaggerResponse(400, "Invalid request payload", typeof(string))]
    [SwaggerResponse(404, "Subscription not found")]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> ActivateSubscription(int id,
        [FromBody] ActivateSubscriptionResource resource, CancellationToken cancellationToken)
    {
        try
        {
            var command = ActivateSubscriptionCommandFromResourceAssembler.ToCommandFromResource(id, resource);
            var result = await subscriptionCommandService.Handle(command, cancellationToken);
            return result switch
            {
                Result<SubscriptionAggregate, ActivateSubscriptionError>.Success success =>
                    Ok(SubscriptionResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
                Result<SubscriptionAggregate, ActivateSubscriptionError>.Failure { Error: ActivateSubscriptionError.SubscriptionNotFound } =>
                    NotFound(),
                _ => Problem(title: "Unexpected error", detail: "Could not activate subscription.", statusCode: 500)
            };
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Validation failed activating subscription {Id}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error activating subscription {Id}", id);
            return Problem(title: "Unexpected server error", detail: "Could not activate subscription.", statusCode: 500);
        }
    }

    /// <summary>Cancels a subscription.</summary>
    [HttpPost("{id:int}/cancel")]
    [SwaggerOperation(Summary = "Cancels a subscription", OperationId = "CancelSubscription")]
    [SwaggerResponse(200, "Subscription cancelled", typeof(SubscriptionResource))]
    [SwaggerResponse(404, "Subscription not found")]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> CancelSubscription(int id, CancellationToken cancellationToken)
    {
        var command = new CancelSubscriptionCommand(new SubscriptionId(id));
        var result = await subscriptionCommandService.Handle(command, cancellationToken);
        return result switch
        {
            Result<SubscriptionAggregate, CancelSubscriptionError>.Success success =>
                Ok(SubscriptionResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
            Result<SubscriptionAggregate, CancelSubscriptionError>.Failure { Error: CancelSubscriptionError.SubscriptionNotFound } =>
                NotFound(),
            _ => Problem(title: "Unexpected error", detail: "Could not cancel subscription.", statusCode: 500)
        };
    }

    /// <summary>Expires a subscription when its end date is reached.</summary>
    [HttpPost("{id:int}/expire")]
    [SwaggerOperation(Summary = "Expires a subscription", OperationId = "ExpireSubscription")]
    [SwaggerResponse(200, "Subscription expired", typeof(SubscriptionResource))]
    [SwaggerResponse(404, "Subscription not found")]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> ExpireSubscription(int id, CancellationToken cancellationToken)
    {
        var command = new ExpireSubscriptionCommand(new SubscriptionId(id));
        var result = await subscriptionCommandService.Handle(command, cancellationToken);
        return result switch
        {
            Result<SubscriptionAggregate, RenewSubscriptionError>.Success success =>
                Ok(SubscriptionResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
            Result<SubscriptionAggregate, RenewSubscriptionError>.Failure { Error: RenewSubscriptionError.SubscriptionNotFound } =>
                NotFound(),
            _ => Problem(title: "Unexpected error", detail: "Could not expire subscription.", statusCode: 500)
        };
    }

    /// <summary>Changes the plan of an existing subscription.</summary>
    [HttpPatch("{id:int}/plan")]
    [SwaggerOperation(Summary = "Changes subscription plan", OperationId = "ChangePlan")]
    [SwaggerResponse(200, "Plan changed", typeof(SubscriptionResource))]
    [SwaggerResponse(400, "Invalid request payload", typeof(string))]
    [SwaggerResponse(404, "Subscription not found")]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> ChangePlan(int id,
        [FromBody] ChangePlanResource resource, CancellationToken cancellationToken)
    {
        try
        {
            var command = ChangePlanCommandFromResourceAssembler.ToCommandFromResource(id, resource);
            var result = await subscriptionCommandService.Handle(command, cancellationToken);
            return result switch
            {
                Result<SubscriptionAggregate, ChangePlanError>.Success success =>
                    Ok(SubscriptionResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
                Result<SubscriptionAggregate, ChangePlanError>.Failure { Error: ChangePlanError.SubscriptionNotFound } =>
                    NotFound(),
                _ => Problem(title: "Unexpected error", detail: "Could not change plan.", statusCode: 500)
            };
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Validation failed changing plan for subscription {Id}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error changing plan for subscription {Id}", id);
            return Problem(title: "Unexpected server error", detail: "Could not change plan.", statusCode: 500);
        }
    }
}
