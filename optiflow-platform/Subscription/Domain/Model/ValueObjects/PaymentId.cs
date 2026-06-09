namespace optiflow_platform.Subscription.Domain.Model.ValueObjects;

/// <summary>
///     Value object representing the identity of a Payment aggregate.
/// </summary>
public sealed record PaymentId
{
    /// <summary>
    ///     Initializes a new <see cref="PaymentId"/> with the given integer value.
    /// </summary>
    /// <param name="value">The payment identifier. Must be a positive integer.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is not positive.</exception>
    public PaymentId(int value)
    {
        if (value <= 0)
            throw new ArgumentException("PaymentId must be a positive integer.", nameof(value));
        Value = value;
    }

    /// <summary>Gets the underlying primitive value.</summary>
    public int Value { get; }

    /// <inheritdoc />
    public override string ToString() => Value.ToString();
}
