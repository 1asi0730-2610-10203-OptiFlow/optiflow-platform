using System.ComponentModel.DataAnnotations;

namespace optiflow_platform.Staff.Interfaces.REST.Resources;

/// <summary>Resource returned for a staff member.</summary>
public record StaffResource(
    int     Id,
    string  EmployeeCode,
    string  FirstName,
    string  LastName,
    string? Email,
    string? Phone,
    string? Role,
    string? Department,
    string  Status,
    bool    ActiveToday,
    string? EntryDate,
    string? Photo);

/// <summary>Resource used to create or update a staff member (matches the frontend payload).</summary>
public record SaveStaffResource(
    string? EmployeeCode,
    [Required] string FirstName,
    string? LastName,
    string? Email,
    string? Phone,
    string? Role,
    string? Department,
    string? Status,
    bool    ActiveToday,
    string? EntryDate,
    string? Photo);
