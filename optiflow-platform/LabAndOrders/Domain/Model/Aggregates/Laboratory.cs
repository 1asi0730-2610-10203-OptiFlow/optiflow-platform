using optiflow_platform.LabAndOrders.Domain.Model.Commands;
using optiflow_platform.LabAndOrders.Domain.Model.ValueObjects;

namespace optiflow_platform.LabAndOrders.Domain.Model.Aggregates;

/// <summary>
///     Laboratory aggregate root representing an external optical laboratory.
/// </summary>
/// <remarks>
///     A laboratory encapsulates its own identity and contact invariants.
///     Names must be unique across the system.
/// </remarks>
public class Laboratory
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected Laboratory()
    {
        Name = null!;
        ContactInfo = null!;
    }

    /// <summary>
    ///     Creates a new laboratory from a registration command.
    /// </summary>
    /// <param name="command">The CreateLaboratoryCommand command.</param>
    /// <exception cref="ArgumentNullException">Thrown when command is null.</exception>
    public Laboratory(CreateLaboratoryCommand command, Guid accountId)
    {
        ArgumentNullException.ThrowIfNull(command);
        AccountId = accountId;
        Name = command.Name;
        ContactInfo = new ContactInfo(command.Phone, command.Email);
    }

    public int Id { get; private set; }
    public Guid AccountId { get; private set; }
    public string Name { get; private set; }
    public ContactInfo ContactInfo { get; private set; }
}
