using Microsoft.EntityFrameworkCore;
using optiflow_platform.LabAndOrders.Application.Errors;
using optiflow_platform.LabAndOrders.Application.Services;
using optiflow_platform.LabAndOrders.Domain.Model.Aggregates;
using optiflow_platform.LabAndOrders.Domain.Model.Commands;
using optiflow_platform.LabAndOrders.Domain.Repositories;
using optiflow_platform.Shared.Application.Patterns;
using optiflow_platform.Shared.Application.Services;
using optiflow_platform.Shared.Domain.Repositories;

namespace optiflow_platform.LabAndOrders.Application.Internal.CommandServices;

/// <summary>
///     Application service for handling laboratory commands.
/// </summary>
/// <remarks>
///     Coordinates laboratory registration, enforcing uniqueness and delegating
///     persistence to the repository and unit of work.
/// </remarks>
/// <param name="laboratoryRepository">Repository for laboratory persistence.</param>
/// <param name="unitOfWork">Unit of work for transaction scope.</param>
/// <param name="logger">Logger for diagnostic and error reporting.</param>
public class LaboratoryCommandService(
    ILaboratoryRepository laboratoryRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserContext currentUserContext,
    ILogger<LaboratoryCommandService> logger)
    : ILaboratoryCommandService
{
    /// <inheritdoc />
    public async Task<Result<Laboratory, CreateLaboratoryError>> Handle(CreateLaboratoryCommand command,
        CancellationToken cancellationToken = default)
    {
        var existing = await laboratoryRepository.FindByNameAsync(command.Name, cancellationToken);
        if (existing is not null)
        {
            logger.LogWarning("Attempted to register a laboratory with duplicate name: {Name}", command.Name);
            return new Result<Laboratory, CreateLaboratoryError>.Failure(CreateLaboratoryError.DuplicateName);
        }

        try
        {
            var laboratory = new Laboratory(command, currentUserContext.AccountId!.Value);
            await laboratoryRepository.AddAsync(laboratory, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<Laboratory, CreateLaboratoryError>.Success(laboratory);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid contact information when registering laboratory {Name}", command.Name);
            return new Result<Laboratory, CreateLaboratoryError>.Failure(CreateLaboratoryError.InvalidContactInfo);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Database error while registering laboratory {Name}", command.Name);
            return new Result<Laboratory, CreateLaboratoryError>.Failure(CreateLaboratoryError.UnexpectedError);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while registering laboratory {Name}", command.Name);
            return new Result<Laboratory, CreateLaboratoryError>.Failure(CreateLaboratoryError.UnexpectedError);
        }
    }
}
