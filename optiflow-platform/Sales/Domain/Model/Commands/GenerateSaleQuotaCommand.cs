namespace optiflow_platform.Sales.Domain.Model.Commands;

public record GenerateSaleQuotaCommand(int SaleId, decimal QuotaAmount);
