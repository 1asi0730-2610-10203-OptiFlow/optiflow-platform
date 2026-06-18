using optiflow_platform.Sales.Domain.Model.ValueObjects;
using optiflow_platform.Shared.Domain.Model.Events;

namespace optiflow_platform.Sales.Domain.Model.Events;

public record DiscountAppliedEvent(SaleId SaleId, string DiscountCode, decimal DiscountAmount, decimal NewTotalAmount) : IEvent;
