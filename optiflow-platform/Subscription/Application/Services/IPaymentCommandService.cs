using optiflow_platform.Shared.Application.Patterns;
using optiflow_platform.Subscription.Application.Errors;
using optiflow_platform.Subscription.Domain.Model.Aggregates;
using optiflow_platform.Subscription.Domain.Model.Commands;

namespace optiflow_platform.Subscription.Application.Services;

/// <summary>Contract for subscription payment command operations.</summary>
public interface IPaymentCommandService
{
    /// <summary>Handles processing a payment for a subscription.</summary>
    Task<Result<Payment, ProcessSubscriptionPaymentError>> Handle(
        ProcessSubscriptionPaymentCommand command, CancellationToken cancellationToken = default);
}
