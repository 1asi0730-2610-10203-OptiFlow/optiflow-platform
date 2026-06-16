using optiflow_platform.Shared.Domain.Model.Events;

namespace optiflow_platform.Sales.Domain.Model.Events;

public record SaleCreatedEvent(int SaleId, string PatientName, decimal TotalAmount) : IEvent;
