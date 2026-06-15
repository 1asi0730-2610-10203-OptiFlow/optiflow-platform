namespace optiflow_platform.Sales.Domain.Model.ValueObjects;

public sealed record SaleStatus
{
    public const string Active = "ACTIVE";
    public const string Partial = "PARTIAL";
    public const string Paid = "PAID";
    public const string Returned = "RETURNED";
    public const string Cancelled = "CANCELLED";
    public const string CancellationRequested = "CANCELLATION_REQUESTED";

    private static readonly HashSet<string> ValidValues =
        [Active, Partial, Paid, Returned, Cancelled, CancellationRequested];

    public string Value { get; }

    public SaleStatus(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("SaleStatus cannot be null or whitespace.", nameof(value));
        if (!ValidValues.Contains(value))
            throw new ArgumentException(
                $"Invalid sale status: {value}. Valid values are: {string.Join(", ", ValidValues)}", nameof(value));
        Value = value;
    }

    public override string ToString() => Value;
}
