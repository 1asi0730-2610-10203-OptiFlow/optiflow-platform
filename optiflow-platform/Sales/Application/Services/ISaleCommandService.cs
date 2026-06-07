using optiflow_platform.Sales.Application.Errors;
using optiflow_platform.Sales.Domain.Model.Aggregates;
using optiflow_platform.Sales.Domain.Model.Commands;
using optiflow_platform.Shared.Application.Patterns;

namespace optiflow_platform.Sales.Application.Services;

/// <summary>
///     Contract for sale command operations.
/// </summary>
public interface ISaleCommandService
{
    /// <summary>Handles the creation of a new sale.</summary>
    Task<Result<Sale, CreateSaleError>> Handle(CreateSaleCommand command,
        CancellationToken cancellationToken = default);

    /// <summary>Handles generating a quota for an existing sale.</summary>
    Task<Result<Sale, GenerateSaleQuotaError>> Handle(GenerateSaleQuotaCommand command,
        CancellationToken cancellationToken = default);

    /// <summary>Handles requesting cancellation of a sale.</summary>
    Task<Result<Sale, RequestSaleCancellationError>> Handle(RequestSaleCancellationCommand command,
        CancellationToken cancellationToken = default);

    /// <summary>Handles cancelling a sale when the lab order has been received.</summary>
    Task<Result<Sale, CancelSaleError>> Handle(CancelSaleCommand command,
        CancellationToken cancellationToken = default);

    /// <summary>Handles applying a promotional discount to a sale.</summary>
    Task<Result<Sale, ApplyPromotionalDiscountError>> Handle(ApplyPromotionalDiscountCommand command,
        CancellationToken cancellationToken = default);

    /// <summary>Handles marking a sale as completed after full payment.</summary>
    Task<Result<Sale, CompleteSaleError>> Handle(CompleteSaleCommand command,
        CancellationToken cancellationToken = default);
}
