namespace optiflow_platform.Inventory.Domain.Model.ValueObjects;

/// <summary>
///     Value object encapsulating the contact information of a supplier.
/// </summary>
public sealed class SupplierContact
{
    /// <summary>
    ///     Private parameterless constructor for EF Core materialization.
    /// </summary>
    private SupplierContact()
    {
        ContactPerson = null!;
        Phone = null!;
        Email = null!;
    }

    /// <summary>
    ///     Creates a new SupplierContact with the given contact person, phone, and email.
    /// </summary>
    /// <param name="contactPerson">The name of the contact person at the supplier.</param>
    /// <param name="phone">The phone number of the supplier.</param>
    /// <param name="email">The email address of the supplier.</param>
    /// <exception cref="ArgumentException">
    ///     Thrown when any of the parameters are null or whitespace,
    ///     or when <paramref name="email"/> is not a valid email format.
    /// </exception>
    public SupplierContact(string contactPerson, string phone, string email)
    {
        if (string.IsNullOrWhiteSpace(contactPerson))
            throw new ArgumentException("Contact person cannot be null or whitespace.", nameof(contactPerson));
        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Phone cannot be null or whitespace.", nameof(phone));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be null or whitespace.", nameof(email));
        if (!email.Contains('@'))
            throw new ArgumentException("Email must be a valid email address.", nameof(email));
        ContactPerson = contactPerson;
        Phone = phone;
        Email = email;
    }

    /// <summary>The name of the contact person at the supplier.</summary>
    public string ContactPerson { get; private set; }

    /// <summary>The contact phone number of the supplier.</summary>
    public string Phone { get; private set; }

    /// <summary>The contact email address of the supplier.</summary>
    public string Email { get; private set; }
}
