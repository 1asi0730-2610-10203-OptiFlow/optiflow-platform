using optiflow_platform.Sales.Domain.Model.ValueObjects;

namespace optiflow_platform.Sales.Domain.Model.Aggregates;

/// <summary>
///     Payment aggregate root tracking balance payments for a sale.
/// </summary>
/// <remarks>
///     Tracks paid and outstanding amounts.
///     Status flow: PENDING → ADVANCED (partial) → COMPLETED (fully cleared).
/// </remarks>
public class Payment
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected Payment()
    {
        Status = null!;
    }

    /// <summary>
    ///     Creates a new payment record for a sale with the given total amount.
    /// </summary>
    public Payment(int saleId, decimal totalAmount)
    {
        SaleId = saleId;
        TotalAmount = totalAmount;
        PaidAmount = 0;
        OutstandingBalance = totalAmount;
        Status = PaymentStatus.Pending;
    }

    public int Id { get; private set; }
    public int SaleId { get; private set; }
    public decimal TotalAmount { get; private set; }
    public decimal PaidAmount { get; private set; }
    public decimal OutstandingBalance { get; private set; }
    public string Status { get; private set; }

    /// <summary>
    ///     Applies a payment amount to the outstanding balance.
    ///     Returns true when the balance is fully cleared.
    /// </summary>
    public bool PayBalance(decimal amount)
    {
        PaidAmount += amount;
        OutstandingBalance = TotalAmount - PaidAmount;
        if (OutstandingBalance <= 0)
        {
            OutstandingBalance = 0;
            Status = PaymentStatus.Completed;
            return true;
        }

        Status = PaymentStatus.Advanced;
        return false;
    }
}
