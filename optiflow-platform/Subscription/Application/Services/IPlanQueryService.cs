using optiflow_platform.Subscription.Domain.Model.Aggregates;
using optiflow_platform.Subscription.Domain.Model.Queries;

namespace optiflow_platform.Subscription.Application.Services;

public interface IPlanQueryService
{
    Task<Plan?> Handle(GetPlanByIdQuery query, CancellationToken cancellationToken = default);
    Task<IEnumerable<Plan>> Handle(GetAllPlansQuery query, CancellationToken cancellationToken = default);
}
