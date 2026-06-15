namespace optiflow_platform.LabAndOrders.Domain.Model.Commands;

/// <summary>
///     Command to register a new optical laboratory.
/// </summary>
/// <param name="Name">The name that identifies the laboratory.</param>
/// <param name="Phone">The contact phone number of the laboratory.</param>
/// <param name="Email">The contact email address of the laboratory.</param>
public record CreateLaboratoryCommand(
    string Name,
    string Phone,
    string Email);