using optiflow_platform.Subscription.Application.Services;
using optiflow_platform.Subscription.Domain.Model.Aggregates;
using optiflow_platform.Subscription.Domain.Model.Queries;
using optiflow_platform.Subscription.Domain.Repositories;

namespace optiflow_platform.Subscription.Application.Internal.QueryServices;

/// <summary>Application service for handling payment queries.</summary>
public class PaymentQueryService(IPaymentRepository paymentRepository)
    : IPaymentQueryService
{
    /// <inheritdoc />
    public async Task<Payment?> Handle(GetPaymentByIdQuery query,
        CancellationToken cancellationToken = default) =>
        await paymentRepository.FindByIdAsync(query.Id.Value, cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<Payment>> Handle(GetPaymentsBySubscriptionIdQuery query,
        CancellationToken cancellationToken = default) =>
        await paymentRepository.FindBySubscriptionIdAsync(query.SubscriptionId, cancellationToken);
}
