using Cortex.Mediator;
using optiflow_platform.Sales.Application.Errors;
using optiflow_platform.Sales.Application.Services;
using optiflow_platform.Sales.Domain.Model.Aggregates;
using optiflow_platform.Sales.Domain.Model.Commands;
using optiflow_platform.Sales.Domain.Model.Events;
using optiflow_platform.Sales.Domain.Repositories;
using optiflow_platform.Shared.Application.Patterns;
using optiflow_platform.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace optiflow_platform.Sales.Application.Internal.CommandServices;

/// <summary>
///     Application service for handling payment commands.
/// </summary>
/// <remarks>
///     When paying the outstanding balance for the first time, a new payment record is created
///     using the sale's current total. Subsequent payments update the existing record.
/// </remarks>
public class PaymentCommandService(
    IPaymentRepository paymentRepository,
    ISaleRepository saleRepository,
    IUnitOfWork unitOfWork,
    IMediator domainEventPublisher,
    ILogger<PaymentCommandService> logger)
    : IPaymentCommandService
{
    /// <inheritdoc />
    public async Task<Result<Payment, PayOutstandingBalanceError>> Handle(PayOutstandingBalanceCommand command,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var payment = await paymentRepository.FindBySaleIdAsync(command.SaleId, cancellationToken);

            if (payment is null)
            {
                var sale = await saleRepository.FindByIdAsync(command.SaleId, cancellationToken);
                if (sale is null)
                {
                    logger.LogWarning("Sale {SaleId} not found while processing payment", command.SaleId);
                    return new Result<Payment, PayOutstandingBalanceError>.Failure(
                        PayOutstandingBalanceError.SaleNotFound);
                }

                payment = new Payment(command.SaleId, sale.TotalAmount);
                payment.PayBalance(command.Amount);
                await paymentRepository.AddAsync(payment, cancellationToken);
            }
            else
            {
                payment.PayBalance(command.Amount);
                paymentRepository.Update(payment);
            }

            await unitOfWork.CompleteAsync(cancellationToken);
            await domainEventPublisher.PublishAsync(
                new OutstandingBalancePayedEvent(payment.SaleId, payment.Id, payment.PaidAmount, payment.OutstandingBalance),
                cancellationToken);
            return new Result<Payment, PayOutstandingBalanceError>.Success(payment);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Database error while processing payment for sale {SaleId}", command.SaleId);
            return new Result<Payment, PayOutstandingBalanceError>.Failure(PayOutstandingBalanceError.UnexpectedError);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while processing payment for sale {SaleId}", command.SaleId);
            return new Result<Payment, PayOutstandingBalanceError>.Failure(PayOutstandingBalanceError.UnexpectedError);
        }
    }
}
