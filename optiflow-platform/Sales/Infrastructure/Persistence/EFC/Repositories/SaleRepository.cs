using optiflow_platform.Sales.Domain.Model.Aggregates;
using optiflow_platform.Sales.Domain.Repositories;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace optiflow_platform.Sales.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     Entity Framework repository for sale persistence.
/// </summary>
/// <param name="context">The EF Core database context.</param>
public class SaleRepository(AppDbContext context)
    : BaseRepository<Sale>(context), ISaleRepository
{
}
