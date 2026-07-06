using System.Linq;
using optiflow_platform.Staff.Domain.Model.Aggregates;
using optiflow_platform.Staff.Domain.Model.Commands;
using optiflow_platform.Staff.Interfaces.REST.Resources;

namespace optiflow_platform.Staff.Interfaces.REST.Transform;

public static class CreateRoleCommandFromResourceAssembler
{
    public static CreateRoleCommand ToCommandFromResource(SaveRoleResource r) =>
        new(r.Name, r.DisplayName, r.Description, r.Color, r.Permissions);
}

public static class UpdateRoleCommandFromResourceAssembler
{
    public static UpdateRoleCommand ToCommandFromResource(int id, SaveRoleResource r) =>
        new(id, r.Name, r.DisplayName, r.Description, r.Color, r.Permissions);
}

public static class RoleResourceFromEntityAssembler
{
    public static RoleResource ToResourceFromEntity(Role e) =>
        new(e.Id, e.Name, e.DisplayName, e.Description, e.Color, e.PermissionList.ToList());
}
