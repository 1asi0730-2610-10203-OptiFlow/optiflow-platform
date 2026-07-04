using optiflow_platform.Inventory.Application.Errors;
using optiflow_platform.Inventory.Application.Services;
using optiflow_platform.Inventory.Domain.Model.Aggregates;
using optiflow_platform.Inventory.Domain.Model.Commands;
using optiflow_platform.Inventory.Domain.Model.ValueObjects;
using optiflow_platform.Inventory.Domain.Repositories;
using optiflow_platform.Shared.Application.Patterns;
using optiflow_platform.Shared.Application.Services;
using optiflow_platform.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace optiflow_platform.Inventory.Application.Internal.CommandServices;

/// <summary>
///     Application service for handling product commands.
/// </summary>
/// <remarks>
///     Handles registration, catalog updates, and stock-changing operations for products.
///     Restocks and manual adjustments additionally record a stock audit log entry as part
///     of the same unit of work.
/// </remarks>
/// <param name="productRepository">Repository for product persistence.</param>
/// <param name="stockAuditLogRepository">Repository for stock audit log persistence.</param>
/// <param name="unitOfWork">Unit of work for transaction scope.</param>
/// <param name="logger">Logger for diagnostic and error reporting.</param>
public class ProductCommandService(
    IProductRepository productRepository,
    IStockAuditLogRepository stockAuditLogRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserContext currentUserContext,
    ILogger<ProductCommandService> logger)
    : IProductCommandService
{
    /// <inheritdoc />
    public async Task<Result<Product, RegisterProductError>> Handle(RegisterProductCommand command,
        CancellationToken cancellationToken = default)
    {
        if (await productRepository.ExistsBySkuAsync(command.Sku, cancellationToken))
        {
            logger.LogWarning("Cannot register product: SKU {Sku} already exists", command.Sku);
            return new Result<Product, RegisterProductError>.Failure(RegisterProductError.DuplicateSku);
        }

        try
        {
            var product = new Product(command, currentUserContext.AccountId!.Value);
            await productRepository.AddAsync(product, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<Product, RegisterProductError>.Success(product);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Database error while registering product with SKU {Sku}", command.Sku);
            return new Result<Product, RegisterProductError>.Failure(RegisterProductError.UnexpectedError);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while registering product with SKU {Sku}", command.Sku);
            return new Result<Product, RegisterProductError>.Failure(RegisterProductError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<Product, UpdateProductError>> Handle(UpdateProductCommand command,
        CancellationToken cancellationToken = default)
    {
        var product = await productRepository.FindByIdAsync(command.ProductId, cancellationToken);
        if (product is null)
        {
            logger.LogWarning("Product {ProductId} not found for catalog update", command.ProductId);
            return new Result<Product, UpdateProductError>.Failure(UpdateProductError.ProductNotFound);
        }

        try
        {
            product.UpdateDetails(command);
            productRepository.Update(product);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<Product, UpdateProductError>.Success(product);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error updating catalog details for product {ProductId}", command.ProductId);
            return new Result<Product, UpdateProductError>.Failure(UpdateProductError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<Product, UpdateStockLevelError>> Handle(UpdateStockLevelCommand command,
        CancellationToken cancellationToken = default)
    {
        var product = await productRepository.FindByIdAsync(command.ProductId, cancellationToken);
        if (product is null)
        {
            logger.LogWarning("Product {ProductId} not found for stock level update", command.ProductId);
            return new Result<Product, UpdateStockLevelError>.Failure(UpdateStockLevelError.ProductNotFound);
        }

        try
        {
            product.UpdateStockLevel(command);
            productRepository.Update(product);
            await unitOfWork.CompleteAsync(cancellationToken);

            if (product.IsLowStock())
                logger.LogWarning("Low stock alert: product {ProductId} ({Sku}) is at {Stock} units, threshold is {Threshold}",
                    product.Id, product.Sku, product.Stock, product.MinimumStockThreshold);

            return new Result<Product, UpdateStockLevelError>.Success(product);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error updating stock level for product {ProductId}", command.ProductId);
            return new Result<Product, UpdateStockLevelError>.Failure(UpdateStockLevelError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<Product, RestockProductError>> Handle(RestockProductCommand command,
        CancellationToken cancellationToken = default)
    {
        var product = await productRepository.FindByIdAsync(command.ProductId, cancellationToken);
        if (product is null)
        {
            logger.LogWarning("Product {ProductId} not found for restock", command.ProductId);
            return new Result<Product, RestockProductError>.Failure(RestockProductError.ProductNotFound);
        }

        try
        {
            var previousStock = product.Stock;
            product.Restock(command);
            productRepository.Update(product);

            var auditLog = new StockAuditLog(product.Id, product.Name, product.Sku, StockOperation.Restock,
                previousStock, command.Quantity, product.Stock, command.Author, product.AccountId);
            await stockAuditLogRepository.AddAsync(auditLog, cancellationToken);

            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<Product, RestockProductError>.Success(product);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid restock quantity {Quantity} for product {ProductId}", command.Quantity, command.ProductId);
            return new Result<Product, RestockProductError>.Failure(RestockProductError.InvalidQuantity);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error restocking product {ProductId}", command.ProductId);
            return new Result<Product, RestockProductError>.Failure(RestockProductError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<Product, ReduceStockError>> Handle(ReduceStockCommand command,
        CancellationToken cancellationToken = default)
    {
        var product = await productRepository.FindByIdAsync(command.ProductId, cancellationToken);
        if (product is null)
        {
            logger.LogWarning("Product {ProductId} not found for stock reduction", command.ProductId);
            return new Result<Product, ReduceStockError>.Failure(ReduceStockError.ProductNotFound);
        }

        try
        {
            var previousStock = product.Stock;
            product.ReduceStock(command.Quantity);
            productRepository.Update(product);

            var auditLog = new StockAuditLog(product.Id, product.Name, product.Sku, StockOperation.Sale,
                previousStock, -command.Quantity, product.Stock, command.Reason, product.AccountId);
            await stockAuditLogRepository.AddAsync(auditLog, cancellationToken);

            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<Product, ReduceStockError>.Success(product);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid reduction quantity {Quantity} for product {ProductId}", command.Quantity, command.ProductId);
            return new Result<Product, ReduceStockError>.Failure(ReduceStockError.InvalidQuantity);
        }
        catch (InvalidOperationException ex) when (ex.Message.StartsWith("Insufficient stock"))
        {
            logger.LogWarning(ex, "Insufficient stock reducing product {ProductId} by {Quantity}", command.ProductId, command.Quantity);
            return new Result<Product, ReduceStockError>.Failure(ReduceStockError.InsufficientStock);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error reducing stock for product {ProductId}", command.ProductId);
            return new Result<Product, ReduceStockError>.Failure(ReduceStockError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<Product, LogManualAdjustmentError>> Handle(LogManualAdjustmentCommand command,
        CancellationToken cancellationToken = default)
    {
        var product = await productRepository.FindByIdAsync(command.ProductId, cancellationToken);
        if (product is null)
        {
            logger.LogWarning("Product {ProductId} not found for manual stock adjustment", command.ProductId);
            return new Result<Product, LogManualAdjustmentError>.Failure(LogManualAdjustmentError.ProductNotFound);
        }

        try
        {
            var previousStock = product.Stock;
            product.AdjustStock(command);
            productRepository.Update(product);

            var auditLog = new StockAuditLog(product.Id, product.Name, product.Sku, StockOperation.ManualAdjustment,
                previousStock, command.NewStock - previousStock, product.Stock, command.Author, product.AccountId);
            await stockAuditLogRepository.AddAsync(auditLog, cancellationToken);

            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<Product, LogManualAdjustmentError>.Success(product);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Missing justification for manual stock adjustment on product {ProductId}", command.ProductId);
            return new Result<Product, LogManualAdjustmentError>.Failure(LogManualAdjustmentError.JustificationRequired);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error logging manual stock adjustment for product {ProductId}", command.ProductId);
            return new Result<Product, LogManualAdjustmentError>.Failure(LogManualAdjustmentError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<Product, VerifySupplyStockError>> Handle(VerifySupplyStockCommand command,
        CancellationToken cancellationToken = default)
    {
        var product = await productRepository.FindByIdAsync(command.ProductId, cancellationToken);
        if (product is null)
        {
            logger.LogWarning("Product {ProductId} not found for supply stock verification", command.ProductId);
            return new Result<Product, VerifySupplyStockError>.Failure(VerifySupplyStockError.ProductNotFound);
        }

        if (!product.HasStock())
        {
            logger.LogWarning("Supply stock verification rejected: product {ProductId} ({Sku}) has no stock available",
                product.Id, product.Sku);
            return new Result<Product, VerifySupplyStockError>.Failure(VerifySupplyStockError.OutOfStock);
        }

        return new Result<Product, VerifySupplyStockError>.Success(product);
    }

    /// <inheritdoc />
    public async Task<Result<Product, ConsumeStockError>> Handle(ConsumeStockCommand command,
        CancellationToken cancellationToken = default)
    {
        var product = await productRepository.FindByIdAsync(command.ProductId, cancellationToken);
        if (product is null)
        {
            logger.LogWarning("Product {ProductId} not found for stock consumption", command.ProductId);
            return new Result<Product, ConsumeStockError>.Failure(ConsumeStockError.ProductNotFound);
        }

        try
        {
            var previousStock = product.Stock;
            product.ConsumeStock(command);
            productRepository.Update(product);

            var auditLog = new StockAuditLog(product.Id, product.Name, product.Sku, StockOperation.Consumption,
                previousStock, -command.Quantity, product.Stock, command.Author, product.AccountId);
            await stockAuditLogRepository.AddAsync(auditLog, cancellationToken);

            await unitOfWork.CompleteAsync(cancellationToken);

            if (product.IsLowStock())
                logger.LogWarning("Low stock alert: product {ProductId} ({Sku}) is at {Stock} units, threshold is {Threshold}",
                    product.Id, product.Sku, product.Stock, product.MinimumStockThreshold);

            return new Result<Product, ConsumeStockError>.Success(product);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid consumption quantity {Quantity} for product {ProductId}", command.Quantity, command.ProductId);
            return new Result<Product, ConsumeStockError>.Failure(ConsumeStockError.InvalidQuantity);
        }
        catch (InvalidOperationException ex) when (ex.Message.StartsWith("Insufficient stock"))
        {
            logger.LogWarning(ex, "Insufficient stock to consume {Quantity} units of product {ProductId}", command.Quantity, command.ProductId);
            return new Result<Product, ConsumeStockError>.Failure(ConsumeStockError.InsufficientStock);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error consuming stock for product {ProductId}", command.ProductId);
            return new Result<Product, ConsumeStockError>.Failure(ConsumeStockError.UnexpectedError);
        }
    }
}
