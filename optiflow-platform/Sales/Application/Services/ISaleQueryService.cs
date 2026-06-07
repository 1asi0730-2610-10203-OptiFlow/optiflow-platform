using optiflow_platform.Sales.Domain.Model.Aggregates;
using optiflow_platform.Sales.Domain.Model.Queries;

namespace optiflow_platform.Sales.Application.Services;

/// <summary>
///     Contract for sale query operations.
/// </summary>
public interface ISaleQueryService
{
    /// <summary>Returns all sales.</summary>
    Task<IEnumerable<Sale>> Handle(GetAllSalesQuery query, CancellationToken cancellationToken = default);

    /// <summary>Returns a single sale by its identifier.</summary>
    Task<Sale?> Handle(GetSaleByIdQuery query, CancellationToken cancellationToken = default);
}
