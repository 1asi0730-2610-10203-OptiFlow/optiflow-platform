using optiflow_platform.Inventory.Application.Errors;
using optiflow_platform.Inventory.Domain.Model.Aggregates;
using optiflow_platform.Inventory.Domain.Model.Commands;
using optiflow_platform.Shared.Application.Patterns;

namespace optiflow_platform.Inventory.Application.Services;

/// <summary>
///     Contract for supplier command operations.
/// </summary>
public interface ISupplierCommandService
{
    /// <summary>
    ///     Handles the registration of a new supplier.
    /// </summary>
    Task<Result<Supplier, CreateSupplierError>> Handle(CreateSupplierCommand command,
        CancellationToken cancellationToken = default);
}
