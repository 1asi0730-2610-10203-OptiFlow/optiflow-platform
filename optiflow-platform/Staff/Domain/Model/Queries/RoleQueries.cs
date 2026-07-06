namespace optiflow_platform.Staff.Domain.Model.Queries;

/// <summary>Query to retrieve all roles of the current optic.</summary>
public record GetAllRolesQuery;

/// <summary>Query to retrieve a role by id.</summary>
public record GetRoleByIdQuery(int Id);
