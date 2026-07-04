using optiflow_platform.Subscription.Domain.Model.Commands;
using optiflow_platform.Subscription.Domain.Model.ValueObjects;

namespace optiflow_platform.Subscription.Domain.Model.Aggregates;

/// <summary>
///     Aggregate recording a subscription payment transaction.
/// </summary>
public class Payment
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core materialisation.
    /// </summary>
    protected Payment()
    {
        PaymentMethod = null!;
        Status        = null!;
    }

    /// <summary>
    ///     Creates a new payment in PENDING status.
    /// </summary>
    public Payment(ProcessSubscriptionPaymentCommand command, Guid accountId)
    {
        ArgumentNullException.ThrowIfNull(command);
        AccountId      = accountId;
        SubscriptionId = command.SubscriptionId;
        Amount         = command.Amount;
        PaymentMethod  = command.PaymentMethod;
        Status         = new PaymentStatus(PaymentStatus.Pending);
        ProcessedAt    = null;
    }

    public int              Id             { get; private set; }
    public Guid             AccountId      { get; private set; }
    public SubscriptionId   SubscriptionId { get; private set; }
    public decimal        Amount         { get; private set; }
    public string         PaymentMethod  { get; private set; }
    public PaymentStatus  Status         { get; private set; }
    public DateTimeOffset? ProcessedAt   { get; private set; }

    /// <summary>
    ///     Marks the payment as processed. The subscription should be activated after this.
    /// </summary>
    public void MarkProcessed()
    {
        Status      = new PaymentStatus(PaymentStatus.Processed);
        ProcessedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>
    ///     Marks the payment as failed. The subscription should be expired if this was a renewal.
    /// </summary>
    public void MarkFailed()
    {
        Status = new PaymentStatus(PaymentStatus.Failed);
    }
}
