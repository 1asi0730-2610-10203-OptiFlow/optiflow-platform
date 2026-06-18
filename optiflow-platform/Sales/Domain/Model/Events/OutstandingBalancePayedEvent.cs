using optiflow_platform.Sales.Domain.Model.ValueObjects;
using optiflow_platform.Shared.Domain.Model.Events;

namespace optiflow_platform.Sales.Domain.Model.Events;

public record OutstandingBalancePayedEvent(SaleId SaleId, int PaymentId, decimal PaidAmount, decimal OutstandingBalance) : IEvent;
