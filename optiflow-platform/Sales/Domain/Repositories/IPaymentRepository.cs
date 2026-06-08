using optiflow_platform.Sales.Domain.Model.Aggregates;
using optiflow_platform.Shared.Domain.Repositories;

namespace optiflow_platform.Sales.Domain.Repositories;

/// <summary>
///     Repository contract for payment persistence.
/// </summary>
public interface IPaymentRepository : IBaseRepository<Payment>
{
    /// <summary>
    ///     Finds the payment associated with a given sale.
    /// </summary>
    Task<Payment?> FindBySaleIdAsync(int saleId, CancellationToken cancellationToken = default);
}
