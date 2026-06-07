using optiflow_platform.Sales.Application.Services;
using optiflow_platform.Sales.Domain.Model.Aggregates;
using optiflow_platform.Sales.Domain.Model.Queries;
using optiflow_platform.Sales.Domain.Repositories;

namespace optiflow_platform.Sales.Application.Internal.QueryServices;

/// <summary>
///     Application service for handling payment queries.
/// </summary>
public class PaymentQueryService(IPaymentRepository paymentRepository) : IPaymentQueryService
{
    /// <inheritdoc />
    public async Task<IEnumerable<Payment>> Handle(GetAllPaymentsQuery query,
        CancellationToken cancellationToken = default) =>
        await paymentRepository.ListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<Payment?> Handle(GetPaymentByIdQuery query, CancellationToken cancellationToken = default) =>
        await paymentRepository.FindByIdAsync(query.Id, cancellationToken);

    /// <inheritdoc />
    public async Task<Payment?> Handle(GetPaymentBySaleIdQuery query,
        CancellationToken cancellationToken = default) =>
        await paymentRepository.FindBySaleIdAsync(query.SaleId, cancellationToken);
}
