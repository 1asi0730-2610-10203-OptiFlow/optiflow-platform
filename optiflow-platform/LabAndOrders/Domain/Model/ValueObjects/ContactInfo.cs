namespace optiflow_platform.LabAndOrders.Domain.Model.ValueObjects;

/// <summary>
///     Value object encapsulating the contact information of a laboratory.
/// </summary>
public sealed class ContactInfo
{
    /// <summary>
    ///     Private parameterless constructor for EF Core materialization.
    /// </summary>
    private ContactInfo()
    {
        Phone = null!;
        Email = null!;
    }

    /// <summary>
    ///     Creates a new ContactInfo with the given phone number and email address.
    /// </summary>
    /// <param name="phone">The phone number of the laboratory.</param>
    /// <param name="email">The email address of the laboratory.</param>
    /// <exception cref="ArgumentException">
    ///     Thrown when <paramref name="phone"/> is null or whitespace,
    ///     or when <paramref name="email"/> is null, whitespace, or not a valid email format.
    /// </exception>
    public ContactInfo(string phone, string email)
    {
        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Phone cannot be null or whitespace.", nameof(phone));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be null or whitespace.", nameof(email));
        if (!email.Contains('@'))
            throw new ArgumentException("Email must be a valid email address.", nameof(email));
        Phone = phone;
        Email = email;
    }

    /// <summary>The contact phone number of the laboratory.</summary>
    public string Phone { get; private set; }

    /// <summary>The contact email address of the laboratory.</summary>
    public string Email { get; private set; }
}
