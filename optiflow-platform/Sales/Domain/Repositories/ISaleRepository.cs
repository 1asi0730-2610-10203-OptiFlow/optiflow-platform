using optiflow_platform.Sales.Domain.Model.Aggregates;
using optiflow_platform.Sales.Domain.Model.ValueObjects;
using optiflow_platform.Shared.Domain.Repositories;

namespace optiflow_platform.Sales.Domain.Repositories;

/// <summary>
///     Repository contract for sale persistence.
/// </summary>
public interface ISaleRepository : IBaseRepository<Sale>
{
    Task<bool> ExistsByInvoiceNumberAsync(InvoiceNumber invoiceNumber, CancellationToken cancellationToken = default);
}
