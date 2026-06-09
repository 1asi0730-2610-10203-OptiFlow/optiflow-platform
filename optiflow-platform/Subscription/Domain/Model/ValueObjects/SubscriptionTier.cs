namespace optiflow_platform.Subscription.Domain.Model.ValueObjects;

public record SubscriptionTier
{
    public const string Basic = "BASiC";
    public const string Professional = "PROFESSIONAL";
    public const string Enterprise = "ENTERPRISE";

    private static readonly HashSet<string> ValidValues = [Basic, Professional, Enterprise];
    
    
    public string Value { get; }

    public SubscriptionTier(string value) {
        if (string.IsNullOrWhiteSpace(value)) {
            throw new ArgumentException("SubscriptionTier cannot be null or whitespace.", nameof(value));
        }
        if (!ValidValues.Contains(value))
            throw new ArgumentException(
                $"Invalid subscription tier: '{value}'. Valid: {string.Join(", ", ValidValues)}",
                nameof(value));
        Value = value;
    }
    
    public override string ToString() => Value;
};