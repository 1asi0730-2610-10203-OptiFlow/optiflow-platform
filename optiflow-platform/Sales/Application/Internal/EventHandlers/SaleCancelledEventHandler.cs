using Humanizer;
using optiflow_platform.Sales.Domain.Model.Events;
using optiflow_platform.Shared.Application.Internal.EventHandlers;

namespace optiflow_platform.Sales.Application.Internal.EventHandlers;

public class SaleCancelledEventHandler : IEventHandler<SaleCancelledEvent> {

    public Task Handle(SaleCancelledEvent domainEvent, CancellationToken cancellationToken)
    {
        return On(domainEvent, cancellationToken);
    }


    private static Task On(SaleCancelledEvent domainEvent, CancellationToken cancellationToken)
    {
        Console.WriteLine("Sale cancelled: " + "Id: " + domainEvent.SaleId);

        return Task.CompletedTask;
    }
}