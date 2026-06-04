namespace optiflow_platform.LabAndOrders.Domain.Model.ValueObjects;

public sealed record OrderStatus
{
    public const string Pending = "PENDING";
    public const string InProduction = "IN_PRODUCTION";
    public const string QualityControl = "QUALITY_CONTROL";
    public const string Ready = "READY";
    public const string Delivered = "DELIVERED";

    private static readonly HashSet<string> ValidValues = [Pending, InProduction, QualityControl, Ready, Delivered];

    public string Value { get; }

    public OrderStatus(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("OrderStatus cannot be null or whitespace.", nameof(value));
        if (!ValidValues.Contains(value))
            throw new ArgumentException($"Invalid order status: {value}. Valid values are: {string.Join(", ", ValidValues)}", nameof(value));
        Value = value;
    }

    public override string ToString() => Value;
}
