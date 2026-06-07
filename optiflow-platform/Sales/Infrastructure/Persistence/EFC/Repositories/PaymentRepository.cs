using optiflow_platform.Sales.Domain.Model.Aggregates;
using optiflow_platform.Sales.Domain.Repositories;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace optiflow_platform.Sales.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     Entity Framework repository for payment persistence.
/// </summary>
/// <param name="context">The EF Core database context.</param>
public class PaymentRepository(AppDbContext context)
    : BaseRepository<Payment>(context), IPaymentRepository
{
    /// <inheritdoc />
    public async Task<Payment?> FindBySaleIdAsync(int saleId, CancellationToken cancellationToken = default) =>
        await Context.Set<Payment>()
            .FirstOrDefaultAsync(p => p.SaleId == saleId, cancellationToken);
}
