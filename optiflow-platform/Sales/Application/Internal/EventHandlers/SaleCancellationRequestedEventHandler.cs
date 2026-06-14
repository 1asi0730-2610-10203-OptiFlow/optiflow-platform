using optiflow_platform.Sales.Application.Services;
using optiflow_platform.Sales.Domain.Model.Commands;
using optiflow_platform.Sales.Domain.Model.Events;
using optiflow_platform.Shared.Application.Internal.EventHandlers;
using optiflow_platform.Sales.Interfaces.Acl;

namespace optiflow_platform.Sales.Application.Internal.EventHandlers;

public class SaleCancellationRequestedEventHandler(ILabAndOrdersContextFacade labAndOrdersContextFacade,
ISaleCommandService saleCommandService) : IEventHandler<SaleCancellationRequestedEvent>
{

    public Task Handle(SaleCancellationRequestedEvent domainEvent, CancellationToken cancellationToken)
    {
        return On(domainEvent, cancellationToken);
    }


    private async Task On(SaleCancellationRequestedEvent domainEvent, CancellationToken cancellationToken) {
        var status = await labAndOrdersContextFacade.FetchWorkOrderStatusBySaleId(domainEvent.SaleId, cancellationToken);

        //// If the order status is in production, throw an error (policy)
        if (status == "IN_PRODUCTION")
        {
            Console.WriteLine("Cannot cancel sale {0}: lab order is in production", domainEvent.SaleId);
            return;
        }
        
        
        await saleCommandService.Handle(new CancelSaleCommand(domainEvent.SaleId, status ?? "NOT_RECEIVED"),  cancellationToken);
        
    }
}