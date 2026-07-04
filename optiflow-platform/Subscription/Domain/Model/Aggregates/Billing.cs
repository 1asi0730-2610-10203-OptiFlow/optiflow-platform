using optiflow_platform.Subscription.Domain.Model.Commands;
using optiflow_platform.Subscription.Domain.Model.ValueObjects;

namespace optiflow_platform.Subscription.Domain.Model.Aggregates;
/// <summary>
/// Why constants instead of a separate Value Object? Billing status has no validation rules beyond membership in a fixed set.
/// Keeping them as string constants on the class itself is simpler and still prevents typos.
/// </summary>
public class Billing {
    public const string StatusPending = "PENDING";
    public const string StatusDue     = "DUE";
    public const string StatusRenewed = "RENEWED";
    public const string StatusExpired = "EXPIRED";

    protected Billing()
    {
        BillingStatus = null!;
    }

    public Billing(CheckSubscriptionRenewalCommand command, Guid accountId)
    {
        ArgumentNullException.ThrowIfNull(command);
        AccountId      = accountId;
        SubscriptionId = command.SubscriptionId;
        BillingStatus  = StatusPending;
        AutoRenew      = true;
        RenewalDate    = DateTimeOffset.UtcNow.AddMonths(1);
    }

    public int              Id             { get; private set; }
    public Guid             AccountId      { get; private set; }
    public SubscriptionId   SubscriptionId { get; private set; }
    public DateTimeOffset  RenewalDate    { get; private set; }
    public bool            AutoRenew      { get; private set; }
    public string          BillingStatus  { get; private set; }

    /// <summary>Called when the scheduler detects this subscription is due for renewal.</summary>
    public void MarkDue() => BillingStatus = StatusDue;

    /// <summary>Called after a successful automatic renewal payment.</summary>
    public void MarkRenewed()
    {
        BillingStatus = StatusRenewed;
        RenewalDate   = DateTimeOffset.UtcNow.AddMonths(1);
    }

    /// <summary>Called when the renewal payment fails.</summary>
    public void MarkExpired() => BillingStatus = StatusExpired;
}

