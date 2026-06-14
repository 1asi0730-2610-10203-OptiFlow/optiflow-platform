namespace optiflow_platform.Sales.Domain.Model.Events;

public record OutstandingBalancePayedEvent(int SaleId, int PaymentId, decimal PaidAmount, decimal OutstandingBalance);
