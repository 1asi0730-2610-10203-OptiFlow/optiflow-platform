using Microsoft.EntityFrameworkCore;
using optiflow_platform.Sales.Domain.Model.Aggregates;
using optiflow_platform.Sales.Domain.Model.ValueObjects;
using optiflow_platform.Sales.Domain.Repositories;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace optiflow_platform.Sales.Infrastructure.Persistence.EFC.Repositories;

public class PaymentRepository(AppDbContext context)
    : BaseRepository<Payment>(context), IPaymentRepository
{
    public override async Task<Payment?> FindByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await Context.Set<Payment>().FirstOrDefaultAsync(p => p.Id == new PaymentId(id), cancellationToken);

    public async Task<Payment?> FindBySaleIdAsync(int saleId, CancellationToken cancellationToken = default) =>
        await Context.Set<Payment>().FirstOrDefaultAsync(p => p.SaleId == saleId, cancellationToken);
}
