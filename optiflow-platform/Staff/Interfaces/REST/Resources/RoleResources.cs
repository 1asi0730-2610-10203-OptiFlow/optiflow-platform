using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace optiflow_platform.Staff.Interfaces.REST.Resources;

/// <summary>Resource returned for a role (matches the frontend role model).</summary>
public record RoleResource(
    int          Id,
    string       Name,
    string       DisplayName,
    string?      Description,
    string       Color,
    List<string> Permissions);

/// <summary>Resource used to create or update a role.</summary>
public record SaveRoleResource(
    [StringLength(50, ErrorMessage = "Name must be at most 50 characters")] string? Name,
    [StringLength(50, ErrorMessage = "DisplayName must be at most 50 characters")] string? DisplayName,
    [StringLength(200, ErrorMessage = "Description must be at most 200 characters")] string? Description,
    [StringLength(20, ErrorMessage = "Color must be at most 20 characters")] string? Color,
    List<string>? Permissions);
