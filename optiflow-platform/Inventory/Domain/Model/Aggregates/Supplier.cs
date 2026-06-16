using optiflow_platform.Inventory.Domain.Model.Commands;
using optiflow_platform.Inventory.Domain.Model.ValueObjects;

namespace optiflow_platform.Inventory.Domain.Model.Aggregates;

/// <summary>
///     Supplier aggregate root representing an external product supplier.
/// </summary>
/// <remarks>
///     A supplier encapsulates its own identity and contact invariants.
///     Names must be unique across the system.
/// </remarks>
public class Supplier
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected Supplier()
    {
        Name = null!;
        Contact = null!;
    }

    /// <summary>
    ///     Creates a new supplier from a registration command.
    /// </summary>
    /// <param name="command">The CreateSupplierCommand command.</param>
    /// <exception cref="ArgumentNullException">Thrown when command is null.</exception>
    public Supplier(CreateSupplierCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        Name = command.Name;
        Contact = new SupplierContact(command.ContactPerson, command.Phone, command.Email);
    }

    public int Id { get; private set; }
    public string Name { get; private set; }
    public SupplierContact Contact { get; private set; }
}
