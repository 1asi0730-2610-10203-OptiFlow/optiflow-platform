using System.Collections.Generic;

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
    string?       Name,
    string?       DisplayName,
    string?       Description,
    string?       Color,
    List<string>? Permissions);
