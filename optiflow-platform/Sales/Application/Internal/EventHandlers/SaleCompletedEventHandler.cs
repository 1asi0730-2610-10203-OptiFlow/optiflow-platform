using optiflow_platform.Sales.Domain.Model.Events;
using optiflow_platform.Shared.Application.Internal.EventHandlers;

namespace optiflow_platform.Sales.Application.Internal.EventHandlers;

public class SaleCompletedEventHandler : IEventHandler<SaleCompletedEvent>
{
    public Task Handle(SaleCompletedEvent domainEvent, CancellationToken cancellationToken)
    {
        return On(domainEvent);
    }

    private static Task On(SaleCompletedEvent domainEvent)
    {
        Console.WriteLine("Sale completed - Id: " + domainEvent.SaleId);
        return Task.CompletedTask;
    }
}
