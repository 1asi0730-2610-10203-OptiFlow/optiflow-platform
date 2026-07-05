namespace optiflow_platform.Staff.Domain.Model.Queries;

/// <summary>Query to retrieve all staff members of the current optic.</summary>
public record GetAllStaffQuery;

/// <summary>Query to retrieve a staff member by id.</summary>
public record GetStaffByIdQuery(int Id);
