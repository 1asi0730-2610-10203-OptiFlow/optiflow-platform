using optiflow_platform.Sales.Domain.Model.Events;
using optiflow_platform.Shared.Application.Internal.EventHandlers;

namespace optiflow_platform.Sales.Application.Internal.EventHandlers;

/// <summary>
///     Handles the <see cref="SaleCreatedEvent"/> domain event.
/// </summary>
/// <remarks>
///     Triggered after a new sale is successfully persisted.
///     Add any side effects here: audit logging, notifications, analytics, etc.
/// </remarks>
public class SaleCreatedEventHandler : IEventHandler<SaleCreatedEvent>
{
    /// <summary>
    ///     Delegates to <see cref="On"/> to process the event.
    /// </summary>
    /// <param name="domainEvent">The event carrying the sale data.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    public Task Handle(SaleCreatedEvent domainEvent, CancellationToken cancellationToken)
    {
        return On(domainEvent);
    }

    /// <summary>
    ///     Executes the side effect triggered by a newly created sale.
    /// </summary>
    /// <param name="domainEvent">
    ///     The event carrying the sale's id, client name and total amount.
    /// </param>
    private static Task On(SaleCreatedEvent domainEvent)
    {
        Console.WriteLine("Sale created — Id: {0}, Patient: {1}, Amount: {2}",
            domainEvent.SaleId,
            domainEvent.PatientName,
            domainEvent.TotalAmount);

        return Task.CompletedTask;
    }
}
