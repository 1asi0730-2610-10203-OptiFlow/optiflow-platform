using optiflow_platform.Sales.Domain.Model.ValueObjects;

namespace optiflow_platform.Sales.Domain.Model.Commands;

public record CancelSaleCommand(SaleId SaleId, string LabOrderStatus);
