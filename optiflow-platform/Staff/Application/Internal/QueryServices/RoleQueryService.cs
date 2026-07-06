using optiflow_platform.Staff.Application.Services;
using optiflow_platform.Staff.Domain.Model.Aggregates;
using optiflow_platform.Staff.Domain.Model.Queries;
using optiflow_platform.Staff.Domain.Repositories;

namespace optiflow_platform.Staff.Application.Internal.QueryServices;

public class RoleQueryService(IRoleRepository roleRepository) : IRoleQueryService
{
    public async Task<IEnumerable<Role>> Handle(GetAllRolesQuery query, CancellationToken cancellationToken = default) =>
        await roleRepository.ListAsync(cancellationToken);

    public async Task<Role?> Handle(GetRoleByIdQuery query, CancellationToken cancellationToken = default) =>
        await roleRepository.FindByIdAsync(query.Id, cancellationToken);
}
