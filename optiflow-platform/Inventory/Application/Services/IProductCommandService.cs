using optiflow_platform.Inventory.Application.Errors;
using optiflow_platform.Inventory.Domain.Model.Aggregates;
using optiflow_platform.Inventory.Domain.Model.Commands;
using optiflow_platform.Shared.Application.Patterns;

namespace optiflow_platform.Inventory.Application.Services;

/// <summary>
///     Contract for product command operations.
/// </summary>
public interface IProductCommandService
{
    /// <summary>Handles the registration of a new product.</summary>
    Task<Result<Product, RegisterProductError>> Handle(RegisterProductCommand command,
        CancellationToken cancellationToken = default);

    /// <summary>Handles updating a product's catalog details.</summary>
    Task<Result<Product, UpdateProductError>> Handle(UpdateProductCommand command,
        CancellationToken cancellationToken = default);

    /// <summary>Handles setting a product's stock to an absolute level.</summary>
    Task<Result<Product, UpdateStockLevelError>> Handle(UpdateStockLevelCommand command,
        CancellationToken cancellationToken = default);

    /// <summary>Handles restocking a product and recording the corresponding audit log.</summary>
    Task<Result<Product, RestockProductError>> Handle(RestockProductCommand command,
        CancellationToken cancellationToken = default);

    /// <summary>Handles reducing a product's stock after a sale and recording the corresponding audit log.</summary>
    Task<Result<Product, ReduceStockError>> Handle(ReduceStockCommand command,
        CancellationToken cancellationToken = default);

    /// <summary>Handles confirming a manual stock adjustment and recording the corresponding audit log.</summary>
    Task<Result<Product, LogManualAdjustmentError>> Handle(LogManualAdjustmentCommand command,
        CancellationToken cancellationToken = default);

    /// <summary>Handles verifying that a product has stock available to satisfy a supply request.</summary>
    Task<Result<Product, VerifySupplyStockError>> Handle(VerifySupplyStockCommand command,
        CancellationToken cancellationToken = default);

    /// <summary>Handles consuming stock to fulfill an external order and recording the corresponding audit log.</summary>
    Task<Result<Product, ConsumeStockError>> Handle(ConsumeStockCommand command,
        CancellationToken cancellationToken = default);
}
