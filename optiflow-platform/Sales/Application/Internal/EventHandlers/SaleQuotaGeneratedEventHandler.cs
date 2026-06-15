using optiflow_platform.Sales.Domain.Model.Events;
using optiflow_platform.Shared.Application.Internal.EventHandlers;

namespace optiflow_platform.Sales.Application.Internal.EventHandlers;

public class SaleQuotaGeneratedEventHandler : IEventHandler<SaleQuotaGeneratedEvent>
{
    public Task Handle(SaleQuotaGeneratedEvent domainEvent, CancellationToken cancellationToken)
    {
        return On(domainEvent);
    }

    private static Task On(SaleQuotaGeneratedEvent domainEvent)
    {
        Console.WriteLine("Sale adelanto set - Id: " + domainEvent.SaleId + ", Adelanto: " + domainEvent.Adelanto);
        return Task.CompletedTask;
    }
}
