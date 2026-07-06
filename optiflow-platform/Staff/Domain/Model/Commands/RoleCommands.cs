using System.Collections.Generic;

namespace optiflow_platform.Staff.Domain.Model.Commands;

/// <summary>Command to create a staff role for the current optic.</summary>
public record CreateRoleCommand(
    string? Name,
    string? DisplayName,
    string? Description,
    string? Color,
    IEnumerable<string>? Permissions);

/// <summary>Command to update an existing role.</summary>
public record UpdateRoleCommand(
    int Id,
    string? Name,
    string? DisplayName,
    string? Description,
    string? Color,
    IEnumerable<string>? Permissions);
