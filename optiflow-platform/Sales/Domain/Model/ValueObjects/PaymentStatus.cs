namespace optiflow_platform.Sales.Domain.Model.ValueObjects;

public sealed record PaymentStatus
{
    public const string Pending = "PENDING";
    public const string Advanced = "ADVANCED";
    public const string Completed = "COMPLETED";

    private static readonly HashSet<string> ValidValues = [Pending, Advanced, Completed];

    public string Value { get; }

    public PaymentStatus(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("PaymentStatus cannot be null or whitespace.", nameof(value));
        if (!ValidValues.Contains(value))
            throw new ArgumentException(
                $"Invalid payment status: {value}. Valid values are: {string.Join(", ", ValidValues)}", nameof(value));
        Value = value;
    }

    public override string ToString() => Value;
}
