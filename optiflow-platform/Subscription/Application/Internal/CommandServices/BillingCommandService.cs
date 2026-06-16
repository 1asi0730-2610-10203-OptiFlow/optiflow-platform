using Cortex.Mediator;
using optiflow_platform.Shared.Application.Patterns;
using optiflow_platform.Shared.Domain.Repositories;
using optiflow_platform.Subscription.Application.Errors;
using optiflow_platform.Subscription.Application.Services;
using optiflow_platform.Subscription.Domain.Model.Aggregates;
using optiflow_platform.Subscription.Domain.Model.Commands;
using optiflow_platform.Subscription.Domain.Model.Events;
using optiflow_platform.Subscription.Domain.Repositories;

namespace optiflow_platform.Subscription.Application.Internal.CommandServices;

/// <summary>Application service for handling billing commands.</summary>
public class BillingCommandService(
    IBillingRepository billingRepository,
    IUnitOfWork unitOfWork,
    IMediator domainEventPublisher,
    ILogger<BillingCommandService> logger)
    : IBillingCommandService
{
    /// <inheritdoc />
    public async Task<Result<Billing, RenewSubscriptionError>> Handle(
        CheckSubscriptionRenewalCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var billing = new Billing(command);
            billing.MarkDue();
            await billingRepository.AddAsync(billing, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            await domainEventPublisher.PublishAsync(
                new RenewalDueEvent(billing.Id, billing.SubscriptionId.Value),
                cancellationToken);
            return new Result<Billing, RenewSubscriptionError>.Success(billing);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error checking renewal for subscription {Id}", command.SubscriptionId.Value);
            return new Result<Billing, RenewSubscriptionError>.Failure(RenewSubscriptionError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<Billing, RenewSubscriptionError>> Handle(
        RequestAutoRenewCommand command, CancellationToken cancellationToken = default)
    {
        var billing = await billingRepository.FindBySubscriptionIdAsync(command.SubscriptionId, cancellationToken);
        if (billing is null)
        {
            logger.LogWarning("Billing not found for subscription {Id}", command.SubscriptionId.Value);
            return new Result<Billing, RenewSubscriptionError>.Failure(RenewSubscriptionError.BillingNotFound);
        }

        try
        {
            billing.MarkRenewed();
            billingRepository.Update(billing);
            await unitOfWork.CompleteAsync(cancellationToken);
            await domainEventPublisher.PublishAsync(
                new RequestedRenewSubscriptionEvent(billing.Id, billing.SubscriptionId.Value),
                cancellationToken);
            return new Result<Billing, RenewSubscriptionError>.Success(billing);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error auto-renewing billing for subscription {Id}", command.SubscriptionId.Value);
            return new Result<Billing, RenewSubscriptionError>.Failure(RenewSubscriptionError.UnexpectedError);
        }
    }
}
