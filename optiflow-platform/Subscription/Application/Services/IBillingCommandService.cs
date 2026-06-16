using optiflow_platform.Shared.Application.Patterns;
using optiflow_platform.Subscription.Application.Errors;
using optiflow_platform.Subscription.Domain.Model.Aggregates;
using optiflow_platform.Subscription.Domain.Model.Commands;

namespace optiflow_platform.Subscription.Application.Services;

/// <summary>Contract for billing command operations.</summary>
public interface IBillingCommandService
{
    /// <summary>Handles checking whether a subscription is due for renewal.</summary>
    Task<Result<Billing, RenewSubscriptionError>> Handle(
        CheckSubscriptionRenewalCommand command, CancellationToken cancellationToken = default);

    /// <summary>Handles processing an automatic renewal charge.</summary>
    Task<Result<Billing, RenewSubscriptionError>> Handle(
        RequestAutoRenewCommand command, CancellationToken cancellationToken = default);
}
