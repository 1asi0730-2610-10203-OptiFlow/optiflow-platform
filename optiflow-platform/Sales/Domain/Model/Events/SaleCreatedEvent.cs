namespace optiflow_platform.Sales.Domain.Model.Events;

public record SaleCreatedEvent(int SaleId, string ClientName, decimal TotalAmount);
