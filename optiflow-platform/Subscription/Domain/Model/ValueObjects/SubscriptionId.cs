namespace optiflow_platform.Subscription.Domain.Model.ValueObjects;

/// <summary>
///     Value object representing the identity of a Subscription aggregate.
/// </summary>
public sealed record SubscriptionId
{
    /// <summary>
    ///     Initializes a new <see cref="SubscriptionId"/> with the given integer value.
    /// </summary>
    /// <param name="value">The subscription identifier. Must be a positive integer.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is not positive.</exception>
    public SubscriptionId(int value)
    {
        if (value <= 0)
            throw new ArgumentException("SubscriptionId must be a positive integer.", nameof(value));
        Value = value;
    }

    /// <summary>Gets the underlying primitive value.</summary>
    public int Value { get; }

    /// <inheritdoc />
    public override string ToString() => Value.ToString();
}
