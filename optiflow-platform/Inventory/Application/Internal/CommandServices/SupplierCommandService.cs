using Microsoft.EntityFrameworkCore;
using optiflow_platform.Inventory.Application.Errors;
using optiflow_platform.Inventory.Application.Services;
using optiflow_platform.Inventory.Domain.Model.Aggregates;
using optiflow_platform.Inventory.Domain.Model.Commands;
using optiflow_platform.Inventory.Domain.Repositories;
using optiflow_platform.Shared.Application.Patterns;
using optiflow_platform.Shared.Application.Services;
using optiflow_platform.Shared.Domain.Repositories;

namespace optiflow_platform.Inventory.Application.Internal.CommandServices;

/// <summary>
///     Application service for handling supplier commands.
/// </summary>
/// <remarks>
///     Coordinates supplier registration, enforcing uniqueness and delegating
///     persistence to the repository and unit of work.
/// </remarks>
/// <param name="supplierRepository">Repository for supplier persistence.</param>
/// <param name="unitOfWork">Unit of work for transaction scope.</param>
/// <param name="logger">Logger for diagnostic and error reporting.</param>
public class SupplierCommandService(
    ISupplierRepository supplierRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserContext currentUserContext,
    ILogger<SupplierCommandService> logger)
    : ISupplierCommandService
{
    /// <inheritdoc />
    public async Task<Result<Supplier, CreateSupplierError>> Handle(CreateSupplierCommand command,
        CancellationToken cancellationToken = default)
    {
        var existing = await supplierRepository.FindByNameAsync(command.Name, cancellationToken);
        if (existing is not null)
        {
            logger.LogWarning("Attempted to register a supplier with duplicate name: {Name}", command.Name);
            return new Result<Supplier, CreateSupplierError>.Failure(CreateSupplierError.DuplicateName);
        }

        try
        {
            var supplier = new Supplier(command, currentUserContext.AccountId!.Value);
            await supplierRepository.AddAsync(supplier, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<Supplier, CreateSupplierError>.Success(supplier);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid contact information when registering supplier {Name}", command.Name);
            return new Result<Supplier, CreateSupplierError>.Failure(CreateSupplierError.InvalidContact);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Database error while registering supplier {Name}", command.Name);
            return new Result<Supplier, CreateSupplierError>.Failure(CreateSupplierError.UnexpectedError);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while registering supplier {Name}", command.Name);
            return new Result<Supplier, CreateSupplierError>.Failure(CreateSupplierError.UnexpectedError);
        }
    }
}
