namespace optiflow_platform.Sales.Domain.Model.Events;

public record OutstandingBalanceClearedEvent(int SaleId, int PaymentId, decimal PaidAmount, decimal OutstandingBalance);
