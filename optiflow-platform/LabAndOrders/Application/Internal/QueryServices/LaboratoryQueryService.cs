using optiflow_platform.LabAndOrders.Application.Services;
using optiflow_platform.LabAndOrders.Domain.Model.Aggregates;
using optiflow_platform.LabAndOrders.Domain.Model.Queries;
using optiflow_platform.LabAndOrders.Domain.Repositories;

namespace optiflow_platform.LabAndOrders.Application.Internal.QueryServices;

/// <summary>
///     Application service for querying laboratories.
/// </summary>
/// <param name="laboratoryRepository">Repository for accessing laboratory data.</param>
public class LaboratoryQueryService(ILaboratoryRepository laboratoryRepository) : ILaboratoryQueryService
{
    /// <inheritdoc />
    public async Task<IEnumerable<Laboratory>> Handle(GetAllLaboratoriesQuery query,
        CancellationToken cancellationToken = default) =>
        await laboratoryRepository.ListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<Laboratory?> Handle(GetLaboratoryByIdQuery query,
        CancellationToken cancellationToken = default) =>
        await laboratoryRepository.FindByIdAsync(query.Id, cancellationToken);
}
