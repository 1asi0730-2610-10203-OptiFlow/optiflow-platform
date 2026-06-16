namespace optiflow_platform.Inventory.Domain.Model.Commands;

/// <summary>
///     Command to register a new supplier.
/// </summary>
/// <param name="Name">The name that identifies the supplier.</param>
/// <param name="ContactPerson">The name of the contact person at the supplier.</param>
/// <param name="Phone">The contact phone number of the supplier.</param>
/// <param name="Email">The contact email address of the supplier.</param>
public record CreateSupplierCommand(
    string Name,
    string ContactPerson,
    string Phone,
    string Email);
