using optiflow_platform.Shared.Domain.Model.Events;

namespace optiflow_platform.Sales.Domain.Model.Events;

public record SaleCancelledEvent(int SaleId) : IEvent;
