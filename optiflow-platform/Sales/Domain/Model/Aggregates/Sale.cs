using optiflow_platform.Sales.Domain.Model.Commands;
using optiflow_platform.Sales.Domain.Model.ValueObjects;

namespace optiflow_platform.Sales.Domain.Model.Aggregates;

public class Sale
{
    private const decimal MinimumQuotaPercentage = 0.30m;

    protected Sale()
    {
        Id = null!;
        InvoiceNumber = null!;
        PatientName = null!;
        UserName = null!;
        PaymentMethod = null!;
        Status = null!;
        CreatedAt = null!;
        LabOrderNumber = string.Empty;
        DiscountCode = string.Empty;
        DeliveredAt = string.Empty;
        Notes = string.Empty;
    }

    public Sale(CreateSaleCommand command, Guid accountId)
    {
        ArgumentNullException.ThrowIfNull(command);
        AccountId = accountId;
        InvoiceNumber = command.InvoiceNumber;
        LabOrderNumber = command.LabOrderNumber ?? string.Empty;
        PatientId = command.PatientId;
        PatientName = command.PatientName;
        UserId = command.UserId;
        UserName = command.UserName;
        TotalAmount = command.TotalAmount;
        Advance = command.Advance;
        PendingBalance = command.TotalAmount - command.Advance;
        DiscountCode = command.DiscountCode ?? string.Empty;
        DiscountAmount = command.DiscountAmount;
        PaymentMethod = command.PaymentMethod;
        Status = SaleStatus.Active;
        CreatedAt = command.CreatedAt;
        DeliveredAt = command.DeliveredAt ?? string.Empty;
        Notes = command.Notes ?? string.Empty;
    }

    public SaleId Id { get; private set; }
    public Guid AccountId { get; private set; }
    public InvoiceNumber InvoiceNumber { get; private set; }
    public string LabOrderNumber { get; private set; }
    public int PatientId { get; private set; }
    public string PatientName { get; private set; }
    public int UserId { get; private set; }
    public string UserName { get; private set; }
    public decimal TotalAmount { get; private set; }
    public decimal Advance { get; private set; }
    public decimal PendingBalance { get; private set; }
    public string DiscountCode { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public string PaymentMethod { get; private set; }
    public string Status { get; private set; }
    public string CreatedAt { get; private set; }
    public string DeliveredAt { get; private set; }
    public string Notes { get; private set; }

    public void GenerateQuota(GenerateSaleQuotaCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        var minimumQuota = TotalAmount * MinimumQuotaPercentage;
        if (command.Advance < minimumQuota)
            throw new ArgumentException(
                $"Advance payment must be at least 30% of the total amount ({minimumQuota:F2}).");
        Advance = command.Advance;
        PendingBalance = TotalAmount - Advance;
        Status = SaleStatus.Partial;
    }

    public void RequestCancellation(RequestSaleCancellationCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        Status = SaleStatus.CancellationRequested;
    }

    /// <exception cref="InvalidOperationException">Thrown when lab order is in production.</exception>
    public void Cancel(CancelSaleCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (command.LabOrderStatus == "IN_PRODUCTION")
            throw new InvalidOperationException("Cannot cancel sale: lab order is currently in production.");
        Status = SaleStatus.Cancelled;
    }

    public void ApplyDiscount(ApplyPromotionalDiscountCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (command.DiscountAmount > TotalAmount)
            throw new ArgumentException(
                $"Discount amount ({command.DiscountAmount:F2}) cannot exceed the sale total ({TotalAmount:F2}).");
        DiscountCode = command.DiscountCode;
        DiscountAmount = command.DiscountAmount;
        TotalAmount -= command.DiscountAmount;
        PendingBalance = TotalAmount - Advance;
    }

    public void Complete()
    {
        PendingBalance = 0;
        Status = SaleStatus.Paid;
    }

    public void RecordPayment(decimal remaining)
    {
        PendingBalance = remaining;
        Status = remaining == 0 ? SaleStatus.Paid : SaleStatus.Partial;
    }
}
