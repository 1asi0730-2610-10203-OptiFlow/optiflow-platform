namespace optiflow_platform.LabAndOrders.Domain.Model.ValueObjects;

public sealed record OrderPriority
{
    public const string Normal = "normal";
    public const string High = "high";
    public const string Urgent = "urgent";

    private static readonly HashSet<string> ValidValues = [Normal, High, Urgent];

    public string Value { get; }

    public OrderPriority(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("OrderPriority cannot be null or whitespace.", nameof(value));
        if (!ValidValues.Contains(value))
            throw new ArgumentException($"Invalid order priority: {value}. Valid values are: {string.Join(", ", ValidValues)}", nameof(value));
        Value = value;
    }

    public override string ToString() => Value;
}
