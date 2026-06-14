namespace optiflow_platform.Sales.Domain.Model.Events;

public record SaleQuotaGeneratedEvent(int SaleId, decimal QuotaAmount);