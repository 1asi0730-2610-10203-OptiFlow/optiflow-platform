using optiflow_platform.Sales.Domain.Model.ValueObjects;

namespace optiflow_platform.Sales.Domain.Model.Commands;

public record PayOutstandingBalanceCommand(SaleId SaleId, decimal AmountPaid, string Method);
