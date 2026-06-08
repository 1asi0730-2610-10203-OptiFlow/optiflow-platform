namespace optiflow_platform.Sales.Domain.Model.Commands;

public record CreateSaleCommand(string ClientName, decimal TotalAmount, string SaleDate);
