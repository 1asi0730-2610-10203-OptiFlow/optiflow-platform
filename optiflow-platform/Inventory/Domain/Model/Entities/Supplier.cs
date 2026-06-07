namespace optiflow_platform.Inventory.Domain.Model.Entities;

/// <summary>
///     Represents an external supplier that provides catalog products.
/// </summary>
public class Supplier
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected Supplier()
    {
        Name = null!;
        ContactPerson = null!;
        Phone = null!;
        Email = null!;
    }

    public int Id { get; private set; }
    public string Name { get; private set; }
    public string ContactPerson { get; private set; }
    public string Phone { get; private set; }
    public string Email { get; private set; }
}
