using optiflow_platform.Staff.Domain.Model.Aggregates;
using optiflow_platform.Staff.Domain.Model.Commands;
using optiflow_platform.Staff.Domain.Model.Queries;

namespace optiflow_platform.Staff.Application.Services;

public interface IRoleCommandService
{
    Task<Role?> Handle(CreateRoleCommand command, CancellationToken cancellationToken = default);
    Task<Role?> Handle(UpdateRoleCommand command, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}

public interface IRoleQueryService
{
    Task<IEnumerable<Role>> Handle(GetAllRolesQuery query, CancellationToken cancellationToken = default);
    Task<Role?> Handle(GetRoleByIdQuery query, CancellationToken cancellationToken = default);
}
