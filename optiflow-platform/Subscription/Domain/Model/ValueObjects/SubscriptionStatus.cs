using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace optiflow_platform.Subscription.Domain.Model.ValueObjects;

public record SubscriptionStatus {

    /**
     * Define states for the sbyscription status;
     */
    public const string PendingPayment = "PENDING_PAYMENT";
    public const string Active = "ACTIVE";
    public const string Cancelled = "CANCELLED";
    public const string Expired = "EXPIRED";
    
    
    private static readonly HashSet<string> validValues= [PendingPayment, Active, Cancelled, Expired];
    
    public string Value { get; }

    public SubscriptionStatus(string value) {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("SbuscriptionStatus cannot be null or whitespace.", nameof(value));
        }

        if (!validValues.Contains(value)) {
            throw new ArgumentException($"Invalid subscription status: `'{value}'. Valid: {string.Join(", ", validValues)}", nameof(value));
        }

        Value = value;
    }
    
    public override string ToString() => Value;
}