namespace optiflow_platform.Sales.Domain.Model.Commands;

public record PayOutstandingBalanceCommand(int SaleId, decimal AmountPaid, string Method);
