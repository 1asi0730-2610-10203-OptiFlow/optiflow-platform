using Microsoft.EntityFrameworkCore;
using optiflow_platform.Shared.Application.Patterns;
using optiflow_platform.Shared.Domain.Repositories;
using optiflow_platform.Subscription.Application.Errors;
using optiflow_platform.Subscription.Application.Services;
using optiflow_platform.Subscription.Domain.Model.Aggregates;
using optiflow_platform.Subscription.Domain.Model.Commands;
using optiflow_platform.Subscription.Domain.Repositories;

namespace optiflow_platform.Subscription.Application.Internal.CommandServices;

/// <summary>Application service for handling subscription commands.</summary>
public class SubscriptionCommandService(
    ISubscriptionRepository subscriptionRepository,
    IUnitOfWork unitOfWork,
    ILogger<SubscriptionCommandService> logger)
    : ISubscriptionCommandService
{
    /// <inheritdoc />
    public async Task<Result<Subscription, SelectSubscriptionPlanError>> Handle(
        SelectSubscriptionPlanCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var subscription = new Subscription(command);
            await subscriptionRepository.AddAsync(subscription, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<Subscription, SelectSubscriptionPlanError>.Success(subscription);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Database error creating subscription for admin {AdminId}", command.AdminId);
            return new Result<Subscription, SelectSubscriptionPlanError>.Failure(SelectSubscriptionPlanError.UnexpectedError);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error creating subscription for admin {AdminId}", command.AdminId);
            return new Result<Subscription, SelectSubscriptionPlanError>.Failure(SelectSubscriptionPlanError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<Subscription, ActivateSubscriptionError>> Handle(
        ActivateSubscriptionCommand command, CancellationToken cancellationToken = default)
    {
        var subscription = await subscriptionRepository.FindByIdAsync(command.SubscriptionId.Value, cancellationToken);
        if (subscription is null)
        {
            logger.LogWarning("Subscription {Id} not found for activation", command.SubscriptionId.Value);
            return new Result<Subscription, ActivateSubscriptionError>.Failure(ActivateSubscriptionError.SubscriptionNotFound);
        }

        try
        {
            subscription.Activate(command);
            subscriptionRepository.Update(subscription);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<Subscription, ActivateSubscriptionError>.Success(subscription);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error activating subscription {Id}", command.SubscriptionId.Value);
            return new Result<Subscription, ActivateSubscriptionError>.Failure(ActivateSubscriptionError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<Subscription, CancelSubscriptionError>> Handle(
        CancelSubscriptionCommand command, CancellationToken cancellationToken = default)
    {
        var subscription = await subscriptionRepository.FindByIdAsync(command.SubscriptionId.Value, cancellationToken);
        if (subscription is null)
        {
            logger.LogWarning("Subscription {Id} not found for cancellation", command.SubscriptionId.Value);
            return new Result<Subscription, CancelSubscriptionError>.Failure(CancelSubscriptionError.SubscriptionNotFound);
        }

        try
        {
            subscription.Cancel(command);
            subscriptionRepository.Update(subscription);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<Subscription, CancelSubscriptionError>.Success(subscription);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error cancelling subscription {Id}", command.SubscriptionId.Value);
            return new Result<Subscription, CancelSubscriptionError>.Failure(CancelSubscriptionError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<Subscription, ChangePlanError>> Handle(
        ChangePlanCommand command, CancellationToken cancellationToken = default)
    {
        var subscription = await subscriptionRepository.FindByIdAsync(command.SubscriptionId.Value, cancellationToken);
        if (subscription is null)
        {
            logger.LogWarning("Subscription {Id} not found for plan change", command.SubscriptionId.Value);
            return new Result<Subscription, ChangePlanError>.Failure(ChangePlanError.SubscriptionNotFound);
        }

        try
        {
            subscription.ChangePlan(command);
            subscriptionRepository.Update(subscription);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<Subscription, ChangePlanError>.Success(subscription);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error changing plan for subscription {Id}", command.SubscriptionId.Value);
            return new Result<Subscription, ChangePlanError>.Failure(ChangePlanError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<Subscription, RenewSubscriptionError>> Handle(
        ExpireSubscriptionCommand command, CancellationToken cancellationToken = default)
    {
        var subscription = await subscriptionRepository.FindByIdAsync(command.SubscriptionId.Value, cancellationToken);
        if (subscription is null)
        {
            logger.LogWarning("Subscription {Id} not found for expiry", command.SubscriptionId.Value);
            return new Result<Subscription, RenewSubscriptionError>.Failure(RenewSubscriptionError.SubscriptionNotFound);
        }

        try
        {
            subscription.Expire(command);
            subscriptionRepository.Update(subscription);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<Subscription, RenewSubscriptionError>.Success(subscription);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error expiring subscription {Id}", command.SubscriptionId.Value);
            return new Result<Subscription, RenewSubscriptionError>.Failure(RenewSubscriptionError.UnexpectedError);
        }
    }
}
