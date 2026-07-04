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
    protected Payment()
    {
        Id = null!;
        Status = null!;
        Method = null!;
        PaidAt = null!;
    }

    public Payment(int saleId, decimal totalAmount, Guid accountId)
    {
        SaleId = saleId;
        AccountId = accountId;
        TotalAmount = totalAmount;
        PaidAmount = 0;
        OutstandingBalance = totalAmount;
        Status = PaymentStatus.Pending;
        Method = string.Empty;
        PaidAt = string.Empty;
    }

    public PaymentId Id { get; private set; }
    public Guid AccountId { get; private set; }
    public int SaleId { get; private set; }
    public decimal TotalAmount { get; private set; }
    public decimal PaidAmount { get; private set; }
    public decimal OutstandingBalance { get; private set; }
    public string Status { get; private set; }
    public string Method { get; private set; }
    public string PaidAt { get; private set; }

    /// <summary>
    ///     Applies a payment toward the outstanding balance.
    ///     Returns true when the balance is fully cleared.
    /// </summary>
    public bool PayBalance(decimal amount, string method)
    {
        PaidAmount += amount;
        OutstandingBalance = TotalAmount - PaidAmount;
        Method = method;
        PaidAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");

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
