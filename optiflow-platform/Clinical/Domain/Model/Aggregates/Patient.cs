using optiflow_platform.Clinical.Domain.Model.Commands;

namespace optiflow_platform.Clinical.Domain.Model.Aggregates;

/// <summary>
///     Patient aggregate root representing a registered optical patient.
/// </summary>
public class Patient
{
    protected Patient()
    {
        FirstName    = null!;
        LastName     = null!;
        Dni          = null!;
        CustomerUuid = null!;
    }

    public Patient(CreatePatientCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        FirstName    = command.FirstName;
        LastName     = command.LastName;
        Dni          = command.Dni;
        Phone        = command.Phone;
        Email        = command.Email;
        BirthDate    = command.BirthDate;
        CustomerUuid = $"pat-{Guid.NewGuid():N}";
    }

    public int      Id           { get; private set; }

    /// <summary>Alias of Id — kept for mockapi compatibility.</summary>
    public int      PatientId    => Id;

    /// <summary>Stable UUID for external references.</summary>
    public string   CustomerUuid { get; private set; }

    public string   FirstName    { get; private set; }
    public string   LastName     { get; private set; }
    public string   Dni          { get; private set; }
    public string?  Phone        { get; private set; }
    public string?  Email        { get; private set; }
    public DateOnly BirthDate    { get; private set; }

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
