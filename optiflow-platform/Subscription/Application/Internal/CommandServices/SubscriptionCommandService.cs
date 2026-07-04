using Cortex.Mediator;
using Microsoft.EntityFrameworkCore;
using optiflow_platform.Shared.Application.Patterns;
using optiflow_platform.Shared.Application.Services;
using optiflow_platform.Shared.Domain.Repositories;
using optiflow_platform.Subscription.Application.Errors;
using optiflow_platform.Subscription.Application.Services;
using optiflow_platform.Subscription.Domain.Model.Commands;
using optiflow_platform.Subscription.Domain.Model.Events;
using optiflow_platform.Subscription.Domain.Repositories;
using SubscriptionAggregate = optiflow_platform.Subscription.Domain.Model.Aggregates.Subscription;

namespace optiflow_platform.Subscription.Application.Internal.CommandServices;

/// <summary>Application service for handling subscription commands.</summary>
public class SubscriptionCommandService(
    ISubscriptionRepository subscriptionRepository,
    IUnitOfWork unitOfWork,
    IMediator domainEventPublisher,
    ICurrentUserContext currentUserContext,
    ILogger<SubscriptionCommandService> logger)
    : ISubscriptionCommandService
{
    /// <inheritdoc />
    public async Task<Result<SubscriptionAggregate, SelectSubscriptionPlanError>> Handle(
        SelectSubscriptionPlanCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var subscription = new SubscriptionAggregate(command, currentUserContext.AccountId!.Value);
            await subscriptionRepository.AddAsync(subscription, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            await domainEventPublisher.PublishAsync(
                new PlanSubscriptionSelectedEvent(subscription.Id, subscription.AdminId, subscription.PlanId.Value),
                cancellationToken);
            return new Result<SubscriptionAggregate, SelectSubscriptionPlanError>.Success(subscription);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Database error creating subscription for admin {AdminId}", command.AdminId);
            return new Result<SubscriptionAggregate, SelectSubscriptionPlanError>.Failure(SelectSubscriptionPlanError.UnexpectedError);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error creating subscription for admin {AdminId}", command.AdminId);
            return new Result<SubscriptionAggregate, SelectSubscriptionPlanError>.Failure(SelectSubscriptionPlanError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<SubscriptionAggregate, ActivateSubscriptionError>> Handle(
        ActivateSubscriptionCommand command, CancellationToken cancellationToken = default)
    {
        var subscription = await subscriptionRepository.FindByIdAsync(command.SubscriptionId.Value, cancellationToken);
        if (subscription is null)
        {
            logger.LogWarning("Subscription {Id} not found for activation", command.SubscriptionId.Value);
            return new Result<SubscriptionAggregate, ActivateSubscriptionError>.Failure(ActivateSubscriptionError.SubscriptionNotFound);
        }

        try
        {
            subscription.Activate(command);
            subscriptionRepository.Update(subscription);
            await unitOfWork.CompleteAsync(cancellationToken);
            await domainEventPublisher.PublishAsync(
                new PaymentProcessingStartedEvent(subscription.Id),
                cancellationToken);
            return new Result<SubscriptionAggregate, ActivateSubscriptionError>.Success(subscription);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error activating subscription {Id}", command.SubscriptionId.Value);
            return new Result<SubscriptionAggregate, ActivateSubscriptionError>.Failure(ActivateSubscriptionError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<SubscriptionAggregate, CancelSubscriptionError>> Handle(
        CancelSubscriptionCommand command, CancellationToken cancellationToken = default)
    {
        var subscription = await subscriptionRepository.FindByIdAsync(command.SubscriptionId.Value, cancellationToken);
        if (subscription is null)
        {
            logger.LogWarning("Subscription {Id} not found for cancellation", command.SubscriptionId.Value);
            return new Result<SubscriptionAggregate, CancelSubscriptionError>.Failure(CancelSubscriptionError.SubscriptionNotFound);
        }

        try
        {
            subscription.Cancel(command);
            subscriptionRepository.Update(subscription);
            await unitOfWork.CompleteAsync(cancellationToken);
            await domainEventPublisher.PublishAsync(
                new SubscriptionCancelledEvent(subscription.Id),
                cancellationToken);
            return new Result<SubscriptionAggregate, CancelSubscriptionError>.Success(subscription);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error cancelling subscription {Id}", command.SubscriptionId.Value);
            return new Result<SubscriptionAggregate, CancelSubscriptionError>.Failure(CancelSubscriptionError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<SubscriptionAggregate, ChangePlanError>> Handle(
        ChangePlanCommand command, CancellationToken cancellationToken = default)
    {
        var subscription = await subscriptionRepository.FindByIdAsync(command.SubscriptionId.Value, cancellationToken);
        if (subscription is null)
        {
            logger.LogWarning("Subscription {Id} not found for plan change", command.SubscriptionId.Value);
            return new Result<SubscriptionAggregate, ChangePlanError>.Failure(ChangePlanError.SubscriptionNotFound);
        }

        try
        {
            subscription.ChangePlan(command);
            subscriptionRepository.Update(subscription);
            await unitOfWork.CompleteAsync(cancellationToken);
            await domainEventPublisher.PublishAsync(
                new PlanChangeRequestSentEvent(subscription.Id, subscription.PlanId.Value),
                cancellationToken);
            return new Result<SubscriptionAggregate, ChangePlanError>.Success(subscription);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error changing plan for subscription {Id}", command.SubscriptionId.Value);
            return new Result<SubscriptionAggregate, ChangePlanError>.Failure(ChangePlanError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<SubscriptionAggregate, RenewSubscriptionError>> Handle(
        ExpireSubscriptionCommand command, CancellationToken cancellationToken = default)
    {
        var subscription = await subscriptionRepository.FindByIdAsync(command.SubscriptionId.Value, cancellationToken);
        if (subscription is null)
        {
            logger.LogWarning("Subscription {Id} not found for expiry", command.SubscriptionId.Value);
            return new Result<SubscriptionAggregate, RenewSubscriptionError>.Failure(RenewSubscriptionError.SubscriptionNotFound);
        }

        try
        {
            subscription.Expire(command);
            subscriptionRepository.Update(subscription);
            await unitOfWork.CompleteAsync(cancellationToken);
            await domainEventPublisher.PublishAsync(
                new SubscriptionExpiredEvent(subscription.Id),
                cancellationToken);
            return new Result<SubscriptionAggregate, RenewSubscriptionError>.Success(subscription);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error expiring subscription {Id}", command.SubscriptionId.Value);
            return new Result<SubscriptionAggregate, RenewSubscriptionError>.Failure(RenewSubscriptionError.UnexpectedError);
        }
    }
}
