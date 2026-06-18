using optiflow_platform.Sales.Domain.Model.ValueObjects;

namespace optiflow_platform.Sales.Domain.Model.Commands;

public record GenerateSaleQuotaCommand(SaleId SaleId, decimal Advance);
