using optiflow_platform.Sales.Domain.Model.Events;
using optiflow_platform.Shared.Application.Internal.EventHandlers;

namespace optiflow_platform.Sales.Application.Internal.EventHandlers;

public class DiscountAppliedEventHandler : IEventHandler<DiscountAppliedEvent>
{
    public Task Handle(DiscountAppliedEvent domainEvent, CancellationToken cancellationToken)
    {
        return On(domainEvent);
    }

    private static Task On(DiscountAppliedEvent domainEvent)
    {
        Console.WriteLine("Discount applied - Id: " + domainEvent.SaleId + ", Code: " + domainEvent.DiscountCode + ", Amount: " + domainEvent.DiscountAmount + ", New total: " + domainEvent.NewTotalAmount);
        return Task.CompletedTask;
    }
}
