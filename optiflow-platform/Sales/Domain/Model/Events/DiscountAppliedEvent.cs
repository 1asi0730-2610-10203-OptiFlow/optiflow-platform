namespace optiflow_platform.Sales.Domain.Model.Events;

public record DiscountAppliedEvent(int SaleId, decimal DiscountPercentage, decimal NewTotalAmount);
