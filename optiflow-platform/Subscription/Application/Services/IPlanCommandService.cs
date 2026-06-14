using optiflow_platform.Shared.Application.Patterns;
using optiflow_platform.Subscription.Application.Errors;
using optiflow_platform.Subscription.Domain.Model.Aggregates;
using optiflow_platform.Subscription.Domain.Model.Commands;

namespace optiflow_platform.Subscription.Application.Services;

public interface IPlanCommandService
{
    Task<Result<Plan, CreatePlanError>> Handle(CreatePlanCommand command, CancellationToken cancellationToken = default);
}
