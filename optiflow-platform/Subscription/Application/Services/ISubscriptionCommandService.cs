using optiflow_platform.Shared.Application.Patterns;
using optiflow_platform.Subscription.Application.Errors;
using optiflow_platform.Subscription.Domain.Model.Commands;
using SubscriptionAggregate = optiflow_platform.Subscription.Domain.Model.Aggregates.Subscription;

namespace optiflow_platform.Subscription.Application.Services;

/// <summary>Contract for subscription command operations.</summary>
public interface ISubscriptionCommandService
{
    /// <summary>Handles selecting a plan and creating a new subscription in PENDING_PAYMENT status.</summary>
    Task<Result<SubscriptionAggregate, SelectSubscriptionPlanError>> Handle(
        SelectSubscriptionPlanCommand command, CancellationToken cancellationToken = default);

    /// <summary>Handles activating a subscription after payment is confirmed.</summary>
    Task<Result<SubscriptionAggregate, ActivateSubscriptionError>> Handle(
        ActivateSubscriptionCommand command, CancellationToken cancellationToken = default);

    /// <summary>Handles explicitly cancelling a subscription.</summary>
    Task<Result<SubscriptionAggregate, CancelSubscriptionError>> Handle(
        CancelSubscriptionCommand command, CancellationToken cancellationToken = default);

    /// <summary>Handles a plan upgrade or downgrade request.</summary>
    Task<Result<SubscriptionAggregate, ChangePlanError>> Handle(
        ChangePlanCommand command, CancellationToken cancellationToken = default);

    /// <summary>Handles expiring a subscription when its end date is reached or renewal fails.</summary>
    Task<Result<SubscriptionAggregate, RenewSubscriptionError>> Handle(
        ExpireSubscriptionCommand command, CancellationToken cancellationToken = default);
}
