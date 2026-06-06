using optiflow_platform.Clinical.Domain.Model.Commands;

namespace optiflow_platform.Clinical.Domain.Model.Aggregates;

/// <summary>
///     Patient aggregate root representing a registered optical patient.
/// </summary>
/// <remarks>
///     A patient holds personal identification data. When a patient is created
///     the system automatically creates a linked <see cref="Entities.ClinicalRecord"/>.
///     Patient data can be updated but the record is never deleted to preserve
///     the full clinical history.
/// </remarks>
public class Patient
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected Patient()
    {
        FirstName = null!;
        LastName  = null!;
        Dni       = null!;
    }

    /// <summary>Creates a new patient from a creation command.</summary>
    public Patient(CreatePatientCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        FirstName = command.FirstName;
        LastName  = command.LastName;
        Dni       = command.Dni;
        Phone     = command.Phone;
        Email     = command.Email;
        BirthDate = command.BirthDate;
    }

    public int      Id        { get; private set; }
    public string   FirstName { get; private set; }
    public string   LastName  { get; private set; }
    public string   Dni       { get; private set; }
    public string?  Phone     { get; private set; }
    public string?  Email     { get; private set; }
    public DateOnly BirthDate { get; private set; }

    /// <summary>Applies an update command to the patient's personal data.</summary>
    public void Update(UpdatePatientCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        FirstName = command.FirstName;
        LastName  = command.LastName;
        Dni       = command.Dni;
        Phone     = command.Phone;
        Email     = command.Email;
        BirthDate = command.BirthDate;
    }
}
