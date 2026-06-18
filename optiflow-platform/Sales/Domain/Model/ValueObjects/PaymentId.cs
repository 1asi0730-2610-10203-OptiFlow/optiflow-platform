namespace optiflow_platform.Sales.Domain.Model.ValueObjects;

public sealed record PaymentId
{
    public PaymentId(int value)
    {
        Value = value;
    }

    public int Value { get; }

    public override string ToString() => Value.ToString();
}
