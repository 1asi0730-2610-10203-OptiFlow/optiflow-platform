using optiflow_platform.Sales.Application.Services;
using optiflow_platform.Sales.Domain.Model.Commands;
using optiflow_platform.Sales.Domain.Model.Events;
using optiflow_platform.Shared.Application.Internal.EventHandlers;

namespace optiflow_platform.Sales.Application.Internal.EventHandlers;

public class OutstandingBalancePayedEventHandler(ISaleCommandService saleCommandService) : IEventHandler<OutstandingBalancePayedEvent>
{
    public Task Handle(OutstandingBalancePayedEvent domainEvent, CancellationToken cancellationToken)
    {
        return On(domainEvent, cancellationToken);
    }

    private async Task On(OutstandingBalancePayedEvent domainEvent, CancellationToken cancellationToken)
    {
        if (domainEvent.OutstandingBalance > 0)
        {
            Console.WriteLine("Payment advanced - SaleId: " + domainEvent.SaleId + ", Remaining: " + domainEvent.OutstandingBalance);
            return;
        }

        Console.WriteLine("Payment completed - SaleId: " + domainEvent.SaleId);
        await saleCommandService.Handle(new CompleteSaleCommand(domainEvent.SaleId), cancellationToken);

    }
}
