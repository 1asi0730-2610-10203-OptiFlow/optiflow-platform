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
    [StringLength(50, ErrorMessage = "EmployeeCode must be at most 50 characters")] string? EmployeeCode,
    [Required] [StringLength(100, ErrorMessage = "FirstName must be at most 100 characters")] string FirstName,
    [StringLength(100, ErrorMessage = "LastName must be at most 100 characters")] string? LastName,
    [EmailAddress] [StringLength(100, ErrorMessage = "Email must be at most 100 characters")] string? Email,
    [Phone] [StringLength(20, ErrorMessage = "Phone must be at most 20 characters")] string? Phone,
    [StringLength(50, ErrorMessage = "Role must be at most 50 characters")] string? Role,
    [StringLength(50, ErrorMessage = "Department must be at most 50 characters")] string? Department,
    [StringLength(30, ErrorMessage = "Status must be at most 30 characters")] string? Status,
    bool    ActiveToday,
    string? EntryDate,
    string? Photo);
