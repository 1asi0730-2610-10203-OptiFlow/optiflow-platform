using System;
using optiflow_platform.Staff.Domain.Model.Commands;

namespace optiflow_platform.Staff.Domain.Model.Aggregates;

/// <summary>
///     A staff member (employee) of an optic. Owned by the optic's account so each optic only sees
///     and manages its own people.
/// </summary>
public class StaffMember
{
    protected StaffMember()
    {
        EmployeeCode = null!;
        FirstName = null!;
        LastName = null!;
        Status = null!;
    }

    public StaffMember(CreateStaffCommand command, Guid accountId)
    {
        ArgumentNullException.ThrowIfNull(command);
        AccountId    = accountId;
        EmployeeCode = string.IsNullOrWhiteSpace(command.EmployeeCode)
            ? $"EMP-{Guid.NewGuid().ToString("N")[..6].ToUpper()}"
            : command.EmployeeCode!;
        FirstName    = command.FirstName;
        LastName     = command.LastName ?? "";
        Email        = command.Email;
        Phone        = command.Phone;
        Role         = command.Role;
        Department   = command.Department;
        Status       = string.IsNullOrWhiteSpace(command.Status) ? "Activo" : command.Status!;
        ActiveToday  = command.ActiveToday;
        EntryDate    = command.EntryDate;
        Photo        = command.Photo;
    }

    public int     Id           { get; private set; }
    public Guid    AccountId    { get; private set; }
    public string  EmployeeCode { get; private set; }
    public string  FirstName    { get; private set; }
    public string  LastName     { get; private set; }
    public string? Email        { get; private set; }
    public string? Phone        { get; private set; }
    public string? Role         { get; private set; }
    public string? Department   { get; private set; }
    public string  Status       { get; private set; }
    public bool    ActiveToday  { get; private set; }
    public string? EntryDate    { get; private set; }
    public string? Photo        { get; private set; }

    public void Update(UpdateStaffCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (!string.IsNullOrWhiteSpace(command.EmployeeCode)) EmployeeCode = command.EmployeeCode!;
        FirstName   = command.FirstName;
        LastName    = command.LastName ?? "";
        Email       = command.Email;
        Phone       = command.Phone;
        Role        = command.Role;
        Department  = command.Department;
        if (!string.IsNullOrWhiteSpace(command.Status)) Status = command.Status!;
        ActiveToday = command.ActiveToday;
        EntryDate   = command.EntryDate;
        Photo       = command.Photo;
    }
}
