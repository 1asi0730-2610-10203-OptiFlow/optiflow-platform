namespace optiflow_platform.LabAndOrders.Domain.Model.Queries;

/// <summary>
///     Query to retrieve a laboratory by its identifier.
/// </summary>
/// <param name="Id">The laboratory identifier.</param>
public record GetLaboratoryByIdQuery(int Id);
