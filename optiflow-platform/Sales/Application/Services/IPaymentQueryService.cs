using optiflow_platform.Sales.Domain.Model.Aggregates;
using optiflow_platform.Sales.Domain.Model.Queries;

namespace optiflow_platform.Sales.Application.Services;

/// <summary>
///     Contract for payment query operations.
/// </summary>
public interface IPaymentQueryService
{
    /// <summary>Returns all payments.</summary>
    Task<IEnumerable<Payment>> Handle(GetAllPaymentsQuery query, CancellationToken cancellationToken = default);

    /// <summary>Returns a single payment by its identifier.</summary>
    Task<Payment?> Handle(GetPaymentByIdQuery query, CancellationToken cancellationToken = default);

    /// <summary>Returns the payment associated with the given sale.</summary>
    Task<Payment?> Handle(GetPaymentBySaleIdQuery query, CancellationToken cancellationToken = default);
}
