namespace optiflow_platform.Sales.Domain.Model.Commands;

public record ApplyPromotionalDiscountCommand(int SaleId, decimal DiscountPercentage);
