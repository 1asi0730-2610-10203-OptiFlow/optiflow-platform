using optiflow_platform.LabAndOrders.Application.Errors;
using optiflow_platform.LabAndOrders.Domain.Model.Aggregates;
using optiflow_platform.LabAndOrders.Domain.Model.Commands;
using optiflow_platform.Shared.Application.Patterns;

namespace optiflow_platform.LabAndOrders.Application.Services;

/// <summary>
///     Contract for laboratory command operations.
/// </summary>
public interface ILaboratoryCommandService
{
    /// <summary>
    ///     Handles the registration of a new laboratory.
    /// </summary>
    Task<Result<Laboratory, CreateLaboratoryError>> Handle(CreateLaboratoryCommand command,
        CancellationToken cancellationToken = default);
}
