using optiflow_platform.Shared.Domain.Model.Events;

namespace optiflow_platform.Sales.Domain.Model.Events;

public record DiscountAppliedEvent(int SaleId, string DiscountCode, decimal DiscountAmount, decimal NewTotalAmount) : IEvent;
