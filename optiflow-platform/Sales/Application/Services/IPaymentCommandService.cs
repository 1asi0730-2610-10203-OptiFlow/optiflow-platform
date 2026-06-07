using optiflow_platform.Sales.Application.Errors;
using optiflow_platform.Sales.Domain.Model.Aggregates;
using optiflow_platform.Sales.Domain.Model.Commands;
using optiflow_platform.Shared.Application.Patterns;

namespace optiflow_platform.Sales.Application.Services;

/// <summary>
///     Contract for payment command operations.
/// </summary>
public interface IPaymentCommandService
{
    /// <summary>Handles paying the outstanding balance of a sale.</summary>
    Task<Result<Payment, PayOutstandingBalanceError>> Handle(PayOutstandingBalanceCommand command,
        CancellationToken cancellationToken = default);
}
