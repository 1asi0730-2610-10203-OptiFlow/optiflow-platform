using optiflow_platform.Sales.Domain.Model.ValueObjects;
using optiflow_platform.Shared.Domain.Model.Events;

namespace optiflow_platform.Sales.Domain.Model.Events;

public record SaleCompletedEvent(SaleId SaleId, string UserName, IReadOnlyList<SaleCompletedItem> Items, Guid AccountId) : IEvent;

/// <summary>
///     A single product and quantity sold as part of a completed sale.
/// </summary>
public record SaleCompletedItem(int ProductId, int Quantity);
