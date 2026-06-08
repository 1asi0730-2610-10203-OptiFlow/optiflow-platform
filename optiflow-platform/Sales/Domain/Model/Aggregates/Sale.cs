using optiflow_platform.Sales.Domain.Model.Commands;
using optiflow_platform.Sales.Domain.Model.ValueObjects;

namespace optiflow_platform.Sales.Domain.Model.Aggregates;

/// <summary>
///     Sale aggregate root representing a commercial transaction.
/// </summary>
/// <remarks>
///     A sale tracks the full lifecycle from creation through quota generation,
///     optional cancellation, discount application, and final completion.
///     Status flow: ACTIVE → QUOTA_GENERATED → CANCELLATION_REQUESTED → CANCELLED or COMPLETED.
/// </remarks>
public class Sale
{
    private const decimal MinimumQuotaPercentage = 0.30m;

    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected Sale()
    {
        ClientName = null!;
        Status = null!;
        SaleDate = null!;
    }

    /// <summary>
    ///     Creates a new sale from a creation command.
    /// </summary>
    public Sale(CreateSaleCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        ClientName = command.ClientName;
        TotalAmount = command.TotalAmount;
        QuotaAmount = 0;
        DiscountPercentage = 0;
        Status = SaleStatus.Active;
        SaleDate = command.SaleDate;
    }

    public int Id { get; private set; }
    public string ClientName { get; private set; }
    public decimal TotalAmount { get; private set; }
    public decimal QuotaAmount { get; private set; }
    public decimal DiscountPercentage { get; private set; }
    public string Status { get; private set; }
    public string SaleDate { get; private set; }

    /// <summary>
    ///     Generates a sale quota. Quota must be at least 30% of the total amount.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when quota is below the 30% minimum.</exception>
    public void GenerateQuota(GenerateSaleQuotaCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        var minimumQuota = TotalAmount * MinimumQuotaPercentage;
        if (command.QuotaAmount < minimumQuota)
            throw new ArgumentException(
                $"Quota amount must be at least 30% of the total amount ({minimumQuota:F2}).");
        QuotaAmount = command.QuotaAmount;
        Status = SaleStatus.QuotaGenerated;
    }

    /// <summary>
    ///     Marks the sale as pending cancellation review.
    /// </summary>
    public void RequestCancellation(RequestSaleCancellationCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        Status = SaleStatus.CancellationRequested;
    }

    /// <summary>
    ///     Cancels the sale. Throws if the lab order is currently in production.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when lab order is in production.</exception>
    public void Cancel(CancelSaleCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (command.LabOrderStatus == "IN_PRODUCTION")
            throw new InvalidOperationException("Cannot cancel sale: lab order is currently in production.");
        Status = SaleStatus.Cancelled;
    }

    /// <summary>
    ///     Applies a promotional discount, reducing the total amount.
    /// </summary>
    public void ApplyDiscount(ApplyPromotionalDiscountCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        DiscountPercentage = command.DiscountPercentage;
        TotalAmount = TotalAmount * (1 - command.DiscountPercentage / 100m);
    }

    /// <summary>
    ///     Marks the sale as completed after full payment.
    /// </summary>
    public void Complete()
    {
        Status = SaleStatus.Completed;
    }
}
