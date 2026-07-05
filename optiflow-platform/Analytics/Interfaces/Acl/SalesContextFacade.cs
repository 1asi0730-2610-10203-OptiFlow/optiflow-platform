using optiflow_platform.Sales.Application.Services;
using optiflow_platform.Sales.Domain.Model.Queries;

namespace optiflow_platform.Analytics.Interfaces.Acl;

/// <summary>
///     Implements <see cref="ISalesContextFacade"/> by delegating to Sales query services.
/// </summary>
/// <remarks>
///     Analytics never imports Sales domain types directly — only this facade does.
/// </remarks>
public class SalesContextFacade(ISaleQueryService saleQueryService) : ISalesContextFacade
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<SaleSummary>> FetchAllSalesAsync(
        CancellationToken cancellationToken = default)
    {
        var sales = await saleQueryService.Handle(new GetAllSalesQuery(), cancellationToken);
        return sales
            .Select(s => new SaleSummary(s.Id.Value, s.CreatedAt, s.TotalAmount, s.PendingBalance, s.Status))
            .ToList();
    }
}
