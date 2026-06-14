using Cortex.Mediator;
using Microsoft.EntityFrameworkCore;
using optiflow_platform.Shared.Application.Patterns;
using optiflow_platform.Shared.Domain.Repositories;
using optiflow_platform.Subscription.Application.Errors;
using optiflow_platform.Subscription.Application.Services;
using optiflow_platform.Subscription.Domain.Model.Aggregates;
using optiflow_platform.Subscription.Domain.Model.Commands;
using optiflow_platform.Subscription.Domain.Model.Events;
using optiflow_platform.Subscription.Domain.Repositories;

namespace optiflow_platform.Subscription.Application.Internal.CommandServices;

/// <summary>Application service for handling payment commands.</summary>
public class PaymentCommandService(
    IPaymentRepository paymentRepository,
    IUnitOfWork unitOfWork,
    IMediator domainEventPublisher,
    ILogger<PaymentCommandService> logger)
    : IPaymentCommandService
{
    /// <inheritdoc />
    public async Task<Result<Payment, ProcessSubscriptionPaymentError>> Handle(
        ProcessSubscriptionPaymentCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var payment = new Payment(command);
            payment.MarkProcessed();
            await paymentRepository.AddAsync(payment, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            await domainEventPublisher.PublishAsync(
                new PaymentProcessedEvent(payment.Id, payment.SubscriptionId.Value, payment.Amount),
                cancellationToken);
            return new Result<Payment, ProcessSubscriptionPaymentError>.Success(payment);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Database error processing payment for subscription {Id}", command.SubscriptionId.Value);
            return new Result<Payment, ProcessSubscriptionPaymentError>.Failure(ProcessSubscriptionPaymentError.UnexpectedError);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error processing payment for subscription {Id}", command.SubscriptionId.Value);
            return new Result<Payment, ProcessSubscriptionPaymentError>.Failure(ProcessSubscriptionPaymentError.UnexpectedError);
        }
    }
}
