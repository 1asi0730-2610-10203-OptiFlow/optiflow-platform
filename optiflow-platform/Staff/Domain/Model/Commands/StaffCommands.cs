namespace optiflow_platform.Staff.Domain.Model.Commands;

/// <summary>Command to register a new staff member (employee) for the current optic.</summary>
public record CreateStaffCommand(
    string? EmployeeCode,
    string  FirstName,
    string? LastName,
    string? Email,
    string? Phone,
    string? Role,
    string? Department,
    string? Status,
    bool    ActiveToday,
    string? EntryDate,
    string? Photo);

/// <summary>Command to update an existing staff member.</summary>
public record UpdateStaffCommand(
    int     Id,
    string? EmployeeCode,
    string  FirstName,
    string? LastName,
    string? Email,
    string? Phone,
    string? Role,
    string? Department,
    string? Status,
    bool    ActiveToday,
    string? EntryDate,
    string? Photo);
