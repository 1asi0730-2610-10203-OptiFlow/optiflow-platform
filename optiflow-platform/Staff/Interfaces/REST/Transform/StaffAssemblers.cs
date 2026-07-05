using optiflow_platform.Staff.Domain.Model.Aggregates;
using optiflow_platform.Staff.Domain.Model.Commands;
using optiflow_platform.Staff.Interfaces.REST.Resources;

namespace optiflow_platform.Staff.Interfaces.REST.Transform;

public static class CreateStaffCommandFromResourceAssembler
{
    public static CreateStaffCommand ToCommandFromResource(SaveStaffResource r) =>
        new(r.EmployeeCode, r.FirstName, r.LastName, r.Email, r.Phone, r.Role, r.Department, r.Status, r.ActiveToday, r.EntryDate, r.Photo);
}

public static class UpdateStaffCommandFromResourceAssembler
{
    public static UpdateStaffCommand ToCommandFromResource(int id, SaveStaffResource r) =>
        new(id, r.EmployeeCode, r.FirstName, r.LastName, r.Email, r.Phone, r.Role, r.Department, r.Status, r.ActiveToday, r.EntryDate, r.Photo);
}

public static class StaffResourceFromEntityAssembler
{
    public static StaffResource ToResourceFromEntity(StaffMember e) =>
        new(e.Id, e.EmployeeCode, e.FirstName, e.LastName, e.Email, e.Phone, e.Role, e.Department, e.Status, e.ActiveToday, e.EntryDate, e.Photo);
}
