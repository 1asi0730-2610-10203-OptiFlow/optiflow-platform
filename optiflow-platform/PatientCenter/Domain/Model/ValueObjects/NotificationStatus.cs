namespace optiflow_platform.PatientCenter.Domain.Model.ValueObjects;

public sealed record NotificationStatus
{
    public const string Pending = "PENDING";
    public const string Sent    = "SENT";
    public const string Read    = "READ";

    private static readonly HashSet<string> ValidValues = [Pending, Sent, Read];

    public string Value { get; }

    public NotificationStatus(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("NotificationStatus cannot be null or whitespace.", nameof(value));
        if (!ValidValues.Contains(value))
            throw new ArgumentException(
                $"Invalid notification status: {value}. Valid values are: {string.Join(", ", ValidValues)}",
                nameof(value));
        Value = value;
    }

    public override string ToString() => Value;
}