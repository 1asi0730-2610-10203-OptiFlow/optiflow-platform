using optiflow_platform.Subscription.Domain.Model.Commands;

namespace optiflow_platform.Subscription.Domain.Model.Aggregates;

public class Billing {
    public const string StatusPending = "PENDING";
    public const string StatusDue     = "DUE";
    public const string StatusRenewed = "RENEWED";
    public const string StatusExpired = "EXPIRED";

    protected Billing()
    {
        BillingStatus = null!;
    }

    public Billing(CheckSubscriptionRenewalCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        SubscriptionId = command.SubscriptionId;
        BillingStatus  = StatusPending;
        AutoRenew      = true;
        RenewalDate    = DateTimeOffset.UtcNow.AddMonths(1);
    }

    public int             Id             { get; private set; }
    public int             SubscriptionId { get; private set; }
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

}