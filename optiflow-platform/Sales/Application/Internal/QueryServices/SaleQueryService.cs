using optiflow_platform.Sales.Application.Services;
using optiflow_platform.Sales.Domain.Model.Aggregates;
using optiflow_platform.Sales.Domain.Model.Queries;
using optiflow_platform.Sales.Domain.Repositories;

namespace optiflow_platform.Sales.Application.Internal.QueryServices;

/// <summary>
///     Application service for handling sale queries.
/// </summary>
public class SaleQueryService(ISaleRepository saleRepository) : ISaleQueryService
{
    /// <inheritdoc />
    public async Task<IEnumerable<Sale>> Handle(GetAllSalesQuery query,
        CancellationToken cancellationToken = default) =>
        await saleRepository.ListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<Sale?> Handle(GetSaleByIdQuery query, CancellationToken cancellationToken = default) =>
        await saleRepository.FindByIdAsync(query.Id, cancellationToken);
}
