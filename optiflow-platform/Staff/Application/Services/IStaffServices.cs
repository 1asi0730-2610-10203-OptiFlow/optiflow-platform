using optiflow_platform.Staff.Domain.Model.Aggregates;
using optiflow_platform.Staff.Domain.Model.Commands;
using optiflow_platform.Staff.Domain.Model.Queries;

namespace optiflow_platform.Staff.Application.Services;

public interface IStaffCommandService
{
    Task<StaffMember?> Handle(CreateStaffCommand command, CancellationToken cancellationToken = default);
    Task<StaffMember?> Handle(UpdateStaffCommand command, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}

public interface IStaffQueryService
{
    Task<IEnumerable<StaffMember>> Handle(GetAllStaffQuery query, CancellationToken cancellationToken = default);
    Task<StaffMember?> Handle(GetStaffByIdQuery query, CancellationToken cancellationToken = default);
}
