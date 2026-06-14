using optiflow_platform.Subscription.Application.Services;
using optiflow_platform.Subscription.Domain.Model.Aggregates;
using optiflow_platform.Subscription.Domain.Model.Queries;
using optiflow_platform.Subscription.Domain.Repositories;

namespace optiflow_platform.Subscription.Application.Internal.QueryServices;

/// <summary>Application service for handling plan queries.</summary>
public class PlanQueryService(IPlanRepository planRepository) : IPlanQueryService
{
    /// <inheritdoc />
    public async Task<Plan?> Handle(GetPlanByIdQuery query, CancellationToken cancellationToken = default) =>
        await planRepository.FindByIdAsync(query.Id.Value, cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<Plan>> Handle(GetAllPlansQuery query, CancellationToken cancellationToken = default) =>
        await planRepository.ListAsync(cancellationToken);
}
