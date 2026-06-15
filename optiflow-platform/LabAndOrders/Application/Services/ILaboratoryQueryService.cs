using optiflow_platform.LabAndOrders.Domain.Model.Aggregates;
using optiflow_platform.LabAndOrders.Domain.Model.Queries;

namespace optiflow_platform.LabAndOrders.Application.Services;

/// <summary>
///     Contract for laboratory query operations.
/// </summary>
public interface ILaboratoryQueryService
{
    /// <summary>
    ///     Handles retrieval of all laboratories.
    /// </summary>
    Task<IEnumerable<Laboratory>> Handle(GetAllLaboratoriesQuery query, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Handles retrieval of a laboratory by its identifier.
    /// </summary>
    Task<Laboratory?> Handle(GetLaboratoryByIdQuery query, CancellationToken cancellationToken = default);
}
