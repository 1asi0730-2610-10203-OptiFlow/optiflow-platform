using optiflow_platform.Sales.Domain.Model.Entities;
using optiflow_platform.Shared.Domain.Repositories;

namespace optiflow_platform.Sales.Domain.Repositories;

/// <summary>
///     Repository contract for sale item persistence.
/// </summary>
public interface ISaleItemRepository : IBaseRepository<SaleItem>
{
    /// <summary>
    ///     Lists the items sold as part of a given sale.
    /// </summary>
    Task<IEnumerable<SaleItem>> ListBySaleIdAsync(int saleId, CancellationToken cancellationToken = default);
}
