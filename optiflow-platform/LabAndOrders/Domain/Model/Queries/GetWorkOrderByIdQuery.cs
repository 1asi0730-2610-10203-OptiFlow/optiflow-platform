namespace optiflow_platform.LabAndOrders.Domain.Model.Queries;

/// <summary>
///     Query to retrieve a work order by its identifier.
/// </summary>
/// <param name="Id">The work order identifier.</param>
public record GetWorkOrderByIdQuery(int Id);
