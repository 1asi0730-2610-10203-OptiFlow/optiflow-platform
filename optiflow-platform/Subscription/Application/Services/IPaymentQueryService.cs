using optiflow_platform.Subscription.Domain.Model.Aggregates;
using optiflow_platform.Subscription.Domain.Model.Queries;

namespace optiflow_platform.Subscription.Application.Services;

/// <summary>Contract for payment query operations.</summary>
public interface IPaymentQueryService
{
    /// <summary>Handles retrieving a payment by its identifier.</summary>
    Task<Payment?> Handle(GetPaymentByIdQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>Handles retrieving all payments for a given subscription.</summary>
    Task<IEnumerable<Payment>> Handle(GetPaymentsBySubscriptionIdQuery query,
        CancellationToken cancellationToken = default);
}
