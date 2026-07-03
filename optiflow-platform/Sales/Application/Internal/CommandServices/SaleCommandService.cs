using Cortex.Mediator;
using optiflow_platform.Sales.Application.Errors;
using optiflow_platform.Sales.Application.Services;
using optiflow_platform.Sales.Domain.Model.Aggregates;
using optiflow_platform.Sales.Domain.Model.Commands;
using optiflow_platform.Sales.Domain.Model.Entities;
using optiflow_platform.Sales.Domain.Model.Events;
using optiflow_platform.Sales.Domain.Repositories;
using optiflow_platform.Shared.Application.Patterns;
using optiflow_platform.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace optiflow_platform.Sales.Application.Internal.CommandServices;

/// <summary>
///     Application service for handling sale commands.
/// </summary>
public class SaleCommandService(
    ISaleRepository saleRepository,
    ISaleItemRepository saleItemRepository,
    IUnitOfWork unitOfWork,
    IMediator domainEventPublisher,
    ILogger<SaleCommandService> logger)
    : ISaleCommandService
{
    /// <inheritdoc />
    public async Task<Result<Sale, CreateSaleError>> Handle(CreateSaleCommand command,
        CancellationToken cancellationToken = default)
    {
        if (await saleRepository.ExistsByInvoiceNumberAsync(command.InvoiceNumber, cancellationToken))
            return new Result<Sale, CreateSaleError>.Failure(CreateSaleError.DuplicateInvoiceNumber);

        try
        {
            var sale = new Sale(command);
            await saleRepository.AddAsync(sale, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);

            foreach (var item in command.Items)
                await saleItemRepository.AddAsync(new SaleItem(sale.Id.Value, item.ProductId, item.Quantity), cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);

            await domainEventPublisher.PublishAsync(new SaleCreatedEvent(sale.Id, sale.PatientName, sale.TotalAmount), cancellationToken);
            return new Result<Sale, CreateSaleError>.Success(sale);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Database error while creating sale for patient {PatientName}", command.PatientName);
            return new Result<Sale, CreateSaleError>.Failure(CreateSaleError.UnexpectedError);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while creating sale for patient {PatientName}", command.PatientName);
            return new Result<Sale, CreateSaleError>.Failure(CreateSaleError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<Sale, GenerateSaleQuotaError>> Handle(GenerateSaleQuotaCommand command,
        CancellationToken cancellationToken = default)
    {
        var sale = await saleRepository.FindByIdAsync(command.SaleId.Value, cancellationToken);
        if (sale is null)
        {
            logger.LogWarning("Sale {SaleId} not found for quota generation", command.SaleId);
            return new Result<Sale, GenerateSaleQuotaError>.Failure(GenerateSaleQuotaError.SaleNotFound);
        }

        try
        {
            sale.GenerateQuota(command);
            saleRepository.Update(sale);
            await unitOfWork.CompleteAsync(cancellationToken);
            await domainEventPublisher.PublishAsync(new SaleQuotaGeneratedEvent(sale.Id, sale.Advance), cancellationToken);
            return new Result<Sale, GenerateSaleQuotaError>.Success(sale);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Quota below minimum for sale {SaleId}", command.SaleId);
            return new Result<Sale, GenerateSaleQuotaError>.Failure(GenerateSaleQuotaError.QuotaBelowMinimum);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error generating quota for sale {SaleId}", command.SaleId);
            return new Result<Sale, GenerateSaleQuotaError>.Failure(GenerateSaleQuotaError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<Sale, RequestSaleCancellationError>> Handle(RequestSaleCancellationCommand command,
        CancellationToken cancellationToken = default)
    {
        var sale = await saleRepository.FindByIdAsync(command.SaleId.Value, cancellationToken);
        if (sale is null)
        {
            logger.LogWarning("Sale {SaleId} not found for cancellation request", command.SaleId);
            return new Result<Sale, RequestSaleCancellationError>.Failure(RequestSaleCancellationError.SaleNotFound);
        }

        try
        {
            sale.RequestCancellation(command);
            saleRepository.Update(sale);
            await unitOfWork.CompleteAsync(cancellationToken);
            await domainEventPublisher.PublishAsync(new SaleCancellationRequestedEvent(sale.Id), cancellationToken);
            return new Result<Sale, RequestSaleCancellationError>.Success(sale);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error requesting cancellation for sale {SaleId}", command.SaleId);
            return new Result<Sale, RequestSaleCancellationError>.Failure(RequestSaleCancellationError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<Sale, CancelSaleError>> Handle(CancelSaleCommand command,
        CancellationToken cancellationToken = default)
    {
        var sale = await saleRepository.FindByIdAsync(command.SaleId.Value, cancellationToken);
        if (sale is null)
        {
            logger.LogWarning("Sale {SaleId} not found for cancellation", command.SaleId);
            return new Result<Sale, CancelSaleError>.Failure(CancelSaleError.SaleNotFound);
        }

        try
        {
            sale.Cancel(command);
            saleRepository.Update(sale);
            await unitOfWork.CompleteAsync(cancellationToken);
            await domainEventPublisher.PublishAsync(new SaleCancelledEvent(sale.Id), cancellationToken);
            return new Result<Sale, CancelSaleError>.Success(sale);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("in production"))
        {
            logger.LogWarning(ex, "Cannot cancel sale {SaleId}: lab order is in production", command.SaleId);
            return new Result<Sale, CancelSaleError>.Failure(CancelSaleError.SaleInProductionStatus);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error cancelling sale {SaleId}", command.SaleId);
            return new Result<Sale, CancelSaleError>.Failure(CancelSaleError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<Sale, ApplyPromotionalDiscountError>> Handle(ApplyPromotionalDiscountCommand command,
        CancellationToken cancellationToken = default)
    {
        var sale = await saleRepository.FindByIdAsync(command.SaleId.Value, cancellationToken);
        if (sale is null)
        {
            logger.LogWarning("Sale {SaleId} not found for discount application", command.SaleId);
            return new Result<Sale, ApplyPromotionalDiscountError>.Failure(ApplyPromotionalDiscountError.SaleNotFound);
        }

        try
        {
            sale.ApplyDiscount(command);
            saleRepository.Update(sale);
            await unitOfWork.CompleteAsync(cancellationToken);
            await domainEventPublisher.PublishAsync(new DiscountAppliedEvent(sale.Id, sale.DiscountCode, sale.DiscountAmount, sale.TotalAmount), cancellationToken);
            return new Result<Sale, ApplyPromotionalDiscountError>.Success(sale);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error applying discount to sale {SaleId}", command.SaleId);
            return new Result<Sale, ApplyPromotionalDiscountError>.Failure(ApplyPromotionalDiscountError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<Sale, CompleteSaleError>> Handle(CompleteSaleCommand command,
        CancellationToken cancellationToken = default)
    {
        var sale = await saleRepository.FindByIdAsync(command.SaleId.Value, cancellationToken);
        if (sale is null)
        {
            logger.LogWarning("Sale {SaleId} not found for completion", command.SaleId);
            return new Result<Sale, CompleteSaleError>.Failure(CompleteSaleError.SaleNotFound);
        }

        try
        {
            sale.Complete();
            saleRepository.Update(sale);
            await unitOfWork.CompleteAsync(cancellationToken);
            await domainEventPublisher.PublishAsync(new SaleCompletedEvent(sale.Id), cancellationToken);
            return new Result<Sale, CompleteSaleError>.Success(sale);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error completing sale {SaleId}", command.SaleId);
            return new Result<Sale, CompleteSaleError>.Failure(CompleteSaleError.UnexpectedError);
        }
    }
}
