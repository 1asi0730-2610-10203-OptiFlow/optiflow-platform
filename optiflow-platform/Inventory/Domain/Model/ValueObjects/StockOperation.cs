namespace optiflow_platform.Inventory.Domain.Model.ValueObjects;

public sealed record StockOperation
{
    public const string Restock = "Restock";
    public const string ManualAdjustment = "Manual Adjustment";
    public const string Sale = "Sale";
    public const string Consumption = "Consumption";

    private static readonly HashSet<string> ValidValues = [Restock, ManualAdjustment, Sale, Consumption];

    public string Value { get; }

    public StockOperation(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("StockOperation cannot be null or whitespace.", nameof(value));
        if (!ValidValues.Contains(value))
            throw new ArgumentException(
                $"Invalid stock operation: {value}. Valid values are: {string.Join(", ", ValidValues)}", nameof(value));
        Value = value;
    }

    public override string ToString() => Value;
}
