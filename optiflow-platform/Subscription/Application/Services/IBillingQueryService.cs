using optiflow_platform.Subscription.Domain.Model.Aggregates;
using optiflow_platform.Subscription.Domain.Model.Queries;

namespace optiflow_platform.Subscription.Application.Services;

/// <summary>Contract for billing query operations.</summary>
public interface IBillingQueryService
{
    /// <summary>Handles retrieving the billing record for a given subscription.</summary>
    Task<Billing?> Handle(GetBillingBySubscriptionIdQuery query,
        CancellationToken cancellationToken = default);
}
