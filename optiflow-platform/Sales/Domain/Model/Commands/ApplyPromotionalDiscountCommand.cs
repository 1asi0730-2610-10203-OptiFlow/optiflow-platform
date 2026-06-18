using optiflow_platform.Sales.Domain.Model.ValueObjects;

namespace optiflow_platform.Sales.Domain.Model.Commands;

public record ApplyPromotionalDiscountCommand(SaleId SaleId, string DiscountCode, decimal DiscountAmount);
