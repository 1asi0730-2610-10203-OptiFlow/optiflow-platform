using optiflow_platform.Sales.Domain.Model.Events;
using optiflow_platform.Shared.Application.Internal.EventHandlers;

namespace optiflow_platform.Sales.Application.Internal.EventHandlers;

public class OutstandingBalanceClearedEventHandler : IEventHandler<OutstandingBalanceClearedEvent>
{
    public Task Handle(OutstandingBalanceClearedEvent domainEvent, CancellationToken cancellationToken)
    {
        return On(domainEvent);
    }

    private static Task On(OutstandingBalanceClearedEvent domainEvent)
    {
        Console.WriteLine("Outstanding balance cleared - SaleId: " + domainEvent.SaleId + ", Paid: " + domainEvent.PaidAmount + ", Remaining: " + domainEvent.OutstandingBalance);
        return Task.CompletedTask;
    }
}
