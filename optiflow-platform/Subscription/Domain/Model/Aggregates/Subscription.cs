using optiflow_platform.Subscription.Domain.Model.Commands;
using optiflow_platform.Subscription.Domain.Model.ValueObjects;

namespace optiflow_platform.Subscription.Domain.Model.Aggregates;

/// <summary>
///     Aggregate root managing the subscription lifecycle:
///     plan selection, activation, cancellation, plan change, and expiry.
/// </summary>
public class Subscription
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core materialisation.
    /// </summary>
    protected Subscription()
    {
        PlanId        = null!;
        Tier          = null!;
        Status        = null!;
        PaymentMethod = null!;
    }

    /// <summary>
    ///     Creates a new subscription in PENDING_PAYMENT status.
    /// </summary>
    public Subscription(SelectSubscriptionPlanCommand command, Guid accountId)
    {
        ArgumentNullException.ThrowIfNull(command);
        AccountId     = accountId;
        AdminId       = command.AdminId;
        PlanId        = command.PlanId;
        Tier          = command.Tier;
        Amount        = command.Amount;
        PaymentMethod = command.PaymentMethod;
        Status        = new SubscriptionStatus(SubscriptionStatus.PendingPayment);
        StartDate     = null;
        EndDate       = null;
    }

    public int               Id            { get; private set; }
    public Guid              AccountId     { get; private set; }
    public int               AdminId       { get; private set; }
    public PlanId            PlanId        { get; private set; }
    public SubscriptionTier  Tier          { get; private set; }
    public decimal           Amount        { get; private set; }
    public string            PaymentMethod { get; private set; }
    public SubscriptionStatus Status       { get; private set; }
    public DateTimeOffset?   StartDate     { get; private set; }
    public DateTimeOffset?   EndDate       { get; private set; }

    /// <summary>
    ///     Transitions to Active and assigns dates and tier after a successful payment.
    /// </summary>
    public void Activate(ActivateSubscriptionCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (command.EndDate <= command.StartDate)
            throw new ArgumentException("Subscription end date must be after the start date.");
        Tier      = command.Tier;
        Status    = new SubscriptionStatus(SubscriptionStatus.Active);
        StartDate = command.StartDate;
        EndDate   = command.EndDate;
    }

    /// <summary>
    ///     Transitions to Cancelled.
    /// </summary>
    public void Cancel(CancelSubscriptionCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        Status = new SubscriptionStatus(SubscriptionStatus.Cancelled);
    }

    /// <summary>
    ///     Transitions to Expired when the end date is reached or a renewal fails.
    /// </summary>
    public void Expire(ExpireSubscriptionCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        Status = new SubscriptionStatus(SubscriptionStatus.Expired);
    }

    /// <summary>
    ///     Applies a plan change and resets to PENDING_PAYMENT until the new charge is confirmed.
    /// </summary>
    public void ChangePlan(ChangePlanCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        PlanId        = command.NewPlanId;
        Tier          = command.NewTier;
        Amount        = command.Amount;
        PaymentMethod = command.PaymentMethod;
        Status        = new SubscriptionStatus(SubscriptionStatus.PendingPayment);
    }
}
