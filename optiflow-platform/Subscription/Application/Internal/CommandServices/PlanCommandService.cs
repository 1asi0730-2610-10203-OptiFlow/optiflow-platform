using Microsoft.EntityFrameworkCore;
using optiflow_platform.Shared.Application.Patterns;
using optiflow_platform.Shared.Domain.Repositories;
using optiflow_platform.Subscription.Application.Errors;
using optiflow_platform.Subscription.Application.Services;
using optiflow_platform.Subscription.Domain.Model.Aggregates;
using optiflow_platform.Subscription.Domain.Model.Commands;
using optiflow_platform.Subscription.Domain.Repositories;

namespace optiflow_platform.Subscription.Application.Internal.CommandServices;

/// <summary>Application service for handling plan commands.</summary>
public class PlanCommandService(
    IPlanRepository planRepository,
    IUnitOfWork unitOfWork,
    ILogger<PlanCommandService> logger)
    : IPlanCommandService
{
    /// <inheritdoc />
    public async Task<Result<Plan, CreatePlanError>> Handle(
        CreatePlanCommand command, CancellationToken cancellationToken = default)
    {
        var existing = await planRepository.FindByNameAsync(command.Name, cancellationToken);
        if (existing is not null)
        {
            logger.LogWarning("Plan with name '{Name}' already exists", command.Name);
            return new Result<Plan, CreatePlanError>.Failure(CreatePlanError.NameAlreadyExists);
        }

        try
        {
            var plan = new Plan(command);
            await planRepository.AddAsync(plan, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<Plan, CreatePlanError>.Success(plan);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Database error creating plan '{Name}'", command.Name);
            return new Result<Plan, CreatePlanError>.Failure(CreatePlanError.UnexpectedError);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error creating plan '{Name}'", command.Name);
            return new Result<Plan, CreatePlanError>.Failure(CreatePlanError.UnexpectedError);
        }
    }
}
