namespace optiflow_platform.Subscription.Domain.Model.ValueObjects;

/// <summary>
///     Value object representing the identity of a Billing aggregate.
/// </summary>
public sealed record BillingId
{
    /// <summary>
    ///     Initializes a new <see cref="BillingId"/> with the given integer value.
    /// </summary>
    /// <param name="value">The billing identifier. Must be a positive integer.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is not positive.</exception>
    public BillingId(int value)
    {
        if (value <= 0)
            throw new ArgumentException("BillingId must be a positive integer.", nameof(value));
        Value = value;
    }

    /// <summary>Gets the underlying primitive value.</summary>
    public int Value { get; }

    /// <inheritdoc />
    public override string ToString() => Value.ToString();
}
