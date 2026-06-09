namespace optiflow_platform.Subscription.Domain.Model.ValueObjects;

public record PaymentStatus
{
    public const string Pending = "PENDING";
    public const string Processed = "PROCESSED";
    public const string Failed = "FAILED";


    private static readonly HashSet<string> ValidValues = [Pending, Processed, Failed];
    
    public string Value { get;  }

    public PaymentStatus(string value) {
        if (string.IsNullOrWhiteSpace(value)) {
            throw new ArgumentException("PaymentStatus cannot be null or whitespace.", nameof(value));
        }

        if (!ValidValues.Contains(value)) {
            throw new ArgumentException($"Invalid Payment Status:  '{value}'. Valid: {string.Join(", ", ValidValues)}", nameof(value));
        }
        Value = value;
    }

    public override string ToString() => Value;
};