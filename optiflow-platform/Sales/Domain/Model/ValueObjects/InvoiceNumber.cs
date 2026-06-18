namespace optiflow_platform.Sales.Domain.Model.ValueObjects;

public sealed record InvoiceNumber
{
    public InvoiceNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("InvoiceNumber cannot be null or whitespace.", nameof(value));
        Value = value.Trim();
    }

    public string Value { get; }

    public override string ToString() => Value;
}
