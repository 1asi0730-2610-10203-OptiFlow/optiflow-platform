using Cortex.Mediator;
using optiflow_platform.Sales.Application.Errors;
using optiflow_platform.Sales.Application.Services;
using optiflow_platform.Sales.Domain.Model.Aggregates;
using optiflow_platform.Sales.Domain.Model.Commands;
using optiflow_platform.Sales.Domain.Model.Events;
using optiflow_platform.Sales.Domain.Model.ValueObjects;
using optiflow_platform.Sales.Domain.Repositories;
using optiflow_platform.Sales.Interfaces.Acl;
using optiflow_platform.Shared.Application.Patterns;
using optiflow_platform.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace optiflow_platform.Sales.Application.Internal.CommandServices;

/// <summary>
///     Application service for handling payment commands.
/// </summary>
/// <remarks>
///     When paying the outstanding balance for the first time, a new payment record is created
///     using the sale's current pending balance (which already accounts for any advance paid
///     at sale creation). Subsequent payments update the existing record. A payment that would
///     fully clear the balance is rejected unless the sale's lab order is READY or DELIVERED —
///     a sale can't close out before its glasses are ready for the patient.
/// </remarks>
public class PaymentCommandService(
    IPaymentRepository paymentRepository,
    ISaleRepository saleRepository,
    ILabAndOrdersContextFacade labAndOrdersContextFacade,
    IUnitOfWork unitOfWork,
    IMediator domainEventPublisher,
    ILogger<PaymentCommandService> logger)
    : IPaymentCommandService
{
    private static readonly string[] CompletableLabOrderStatuses = ["READY", "DELIVERED"];

    /// <inheritdoc />
    public async Task<Result<Payment, PayOutstandingBalanceError>> Handle(PayOutstandingBalanceCommand command,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var payment = await paymentRepository.FindBySaleIdAsync(command.SaleId.Value, cancellationToken);
            var sale = await saleRepository.FindByIdAsync(command.SaleId.Value, cancellationToken);
            if (sale is null)
            {
                logger.LogWarning("Sale {SaleId} not found while processing payment", command.SaleId);
                return new Result<Payment, PayOutstandingBalanceError>.Failure(
                    PayOutstandingBalanceError.SaleNotFound);
            }

            var wouldFullyClearBalance = sale.PendingBalance - command.AmountPaid <= 0;
            if (wouldFullyClearBalance)
            {
                var labOrderStatus = await labAndOrdersContextFacade
                    .FetchWorkOrderStatusBySaleId(sale.Id.Value, cancellationToken);
                if (labOrderStatus is null || !CompletableLabOrderStatuses.Contains(labOrderStatus))
                {
                    logger.LogWarning(
                        "Rejected payment that would complete sale {SaleId}: lab order status is {Status}",
                        command.SaleId, labOrderStatus ?? "NOT_FOUND");
                    return new Result<Payment, PayOutstandingBalanceError>.Failure(
                        PayOutstandingBalanceError.LabOrderNotReady);
                }
            }

            if (payment is null)
            {
                payment = new Payment(command.SaleId.Value, sale.PendingBalance);
                payment.PayBalance(command.AmountPaid, command.Method);
                await paymentRepository.AddAsync(payment, cancellationToken);
            }
            else
            {
                payment.PayBalance(command.AmountPaid, command.Method);
                paymentRepository.Update(payment);
            }

            sale.RecordPayment(payment.OutstandingBalance);
            saleRepository.Update(sale);

            await unitOfWork.CompleteAsync(cancellationToken);
            await domainEventPublisher.PublishAsync(
                new OutstandingBalancePayedEvent(new SaleId(payment.SaleId), payment.Id, payment.PaidAmount, payment.OutstandingBalance),
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
