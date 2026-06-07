namespace optiflow_platform.Sales.Domain.Model.Commands;

public record CancelSaleCommand(int SaleId, string LabOrderStatus);
