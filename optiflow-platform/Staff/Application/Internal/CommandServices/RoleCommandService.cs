using optiflow_platform.Shared.Application.Services;
using optiflow_platform.Shared.Domain.Repositories;
using optiflow_platform.Staff.Application.Services;
using optiflow_platform.Staff.Domain.Model.Aggregates;
using optiflow_platform.Staff.Domain.Model.Commands;
using optiflow_platform.Staff.Domain.Repositories;

namespace optiflow_platform.Staff.Application.Internal.CommandServices;

public class RoleCommandService(
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserContext currentUserContext) : IRoleCommandService
{
    public async Task<Role?> Handle(CreateRoleCommand command, CancellationToken cancellationToken = default)
    {
        if (currentUserContext.AccountId is not { } accountId) return null;
        var role = new Role(command, accountId);
        await roleRepository.AddAsync(role, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return role;
    }

    public async Task<Role?> Handle(UpdateRoleCommand command, CancellationToken cancellationToken = default)
    {
        var role = await roleRepository.FindByIdAsync(command.Id, cancellationToken);
        if (role is null) return null;
        role.Update(command);
        roleRepository.Update(role);
        await unitOfWork.CompleteAsync(cancellationToken);
        return role;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var role = await roleRepository.FindByIdAsync(id, cancellationToken);
        if (role is null) return false;
        roleRepository.Remove(role);
        await unitOfWork.CompleteAsync(cancellationToken);
        return true;
    }
}
