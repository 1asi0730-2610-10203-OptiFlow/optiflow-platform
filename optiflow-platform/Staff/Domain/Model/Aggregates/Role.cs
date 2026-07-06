using System;
using System.Collections.Generic;
using System.Linq;
using optiflow_platform.Staff.Domain.Model.Commands;

namespace optiflow_platform.Staff.Domain.Model.Aggregates;

/// <summary>
///     A staff role (job role) of an optic — e.g. Optometrista, Recepcionista. Owned by the optic's
///     account. Permissions are stored comma-joined and exposed as a list.
/// </summary>
public class Role
{
    protected Role()
    {
        Name = null!;
        DisplayName = null!;
        Permissions = "";
    }

    public Role(CreateRoleCommand command, Guid accountId)
    {
        ArgumentNullException.ThrowIfNull(command);
        AccountId   = accountId;
        DisplayName = string.IsNullOrWhiteSpace(command.DisplayName) ? (command.Name ?? "Rol") : command.DisplayName!;
        Name        = string.IsNullOrWhiteSpace(command.Name) ? Internalize(DisplayName) : command.Name!;
        Description = command.Description;
        Color       = string.IsNullOrWhiteSpace(command.Color) ? "#64748b" : command.Color!;
        Permissions = JoinPermissions(command.Permissions);
    }

    public int     Id          { get; private set; }
    public Guid    AccountId   { get; private set; }
    public string  Name        { get; private set; }
    public string  DisplayName { get; private set; }
    public string? Description  { get; private set; }
    public string  Color       { get; private set; } = "#64748b";
    public string  Permissions { get; private set; } = "";

    public IEnumerable<string> PermissionList =>
        string.IsNullOrWhiteSpace(Permissions) ? [] : Permissions.Split(',', StringSplitOptions.RemoveEmptyEntries);

    public void Update(UpdateRoleCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (!string.IsNullOrWhiteSpace(command.DisplayName)) DisplayName = command.DisplayName!;
        if (!string.IsNullOrWhiteSpace(command.Name)) Name = command.Name!;
        Description = command.Description;
        if (!string.IsNullOrWhiteSpace(command.Color)) Color = command.Color!;
        Permissions = JoinPermissions(command.Permissions);
    }

    private static string JoinPermissions(IEnumerable<string>? permissions) =>
        permissions is null ? "" : string.Join(',', permissions.Where(p => !string.IsNullOrWhiteSpace(p)));

    private static string Internalize(string displayName) =>
        displayName.Trim().ToUpperInvariant().Replace(' ', '_');
}
