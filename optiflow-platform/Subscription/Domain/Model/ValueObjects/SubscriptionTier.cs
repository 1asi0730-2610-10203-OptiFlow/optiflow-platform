namespace optiflow_platform.Subscription.Domain.Model.ValueObjects;

public record SubscriptionTier
{
    public const string Basic = "BASIC";
    public const string Professional = "PROFESSIONAL";
    public const string Enterprise = "ENTERPRISE";

    private static readonly HashSet<string> ValidValues = [Basic, Professional, Enterprise];


    public string Value { get; }

    public SubscriptionTier(string value) {
        if (string.IsNullOrWhiteSpace(value)) {
            throw new ArgumentException("SubscriptionTier cannot be null or whitespace.", nameof(value));
        }
        // Normalize so callers can pass "Basic"/"basic" and, critically, so rows persisted under the
        // former "BASiC" typo still load (this runs on every EF read via the value converter).
        var normalized = value.Trim().ToUpperInvariant();
        if (!ValidValues.Contains(normalized))
            throw new ArgumentException(
                $"Invalid subscription tier: '{value}'. Valid: {string.Join(", ", ValidValues)}",
                nameof(value));
        Value = normalized;
    }
    
    public override string ToString() => Value;
};