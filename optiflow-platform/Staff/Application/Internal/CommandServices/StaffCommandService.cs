using optiflow_platform.Shared.Application.Services;
using optiflow_platform.Shared.Domain.Repositories;
using optiflow_platform.Staff.Application.Services;
using optiflow_platform.Staff.Domain.Model.Aggregates;
using optiflow_platform.Staff.Domain.Model.Commands;
using optiflow_platform.Staff.Domain.Repositories;

namespace optiflow_platform.Staff.Application.Internal.CommandServices;

public class StaffCommandService(
    IStaffRepository staffRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserContext currentUserContext) : IStaffCommandService
{
    public async Task<StaffMember?> Handle(CreateStaffCommand command, CancellationToken cancellationToken = default)
    {
        if (currentUserContext.AccountId is not { } accountId) return null;
        var staff = new StaffMember(command, accountId);
        await staffRepository.AddAsync(staff, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return staff;
    }

    public async Task<StaffMember?> Handle(UpdateStaffCommand command, CancellationToken cancellationToken = default)
    {
        var staff = await staffRepository.FindByIdAsync(command.Id, cancellationToken);
        if (staff is null) return null;
        staff.Update(command);
        staffRepository.Update(staff);
        await unitOfWork.CompleteAsync(cancellationToken);
        return staff;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var staff = await staffRepository.FindByIdAsync(id, cancellationToken);
        if (staff is null) return false;
        staffRepository.Remove(staff);
        await unitOfWork.CompleteAsync(cancellationToken);
        return true;
    }
}
