namespace optiflow_platform.Sales.Domain.Model.Commands;

public record ApplyPromotionalDiscountCommand(int SaleId, string DiscountCode, decimal DiscountAmount);
