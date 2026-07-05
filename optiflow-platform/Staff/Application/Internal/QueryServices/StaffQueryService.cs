using optiflow_platform.Staff.Application.Services;
using optiflow_platform.Staff.Domain.Model.Aggregates;
using optiflow_platform.Staff.Domain.Model.Queries;
using optiflow_platform.Staff.Domain.Repositories;

namespace optiflow_platform.Staff.Application.Internal.QueryServices;

public class StaffQueryService(IStaffRepository staffRepository) : IStaffQueryService
{
    public async Task<IEnumerable<StaffMember>> Handle(GetAllStaffQuery query, CancellationToken cancellationToken = default) =>
        await staffRepository.ListAsync(cancellationToken);

    public async Task<StaffMember?> Handle(GetStaffByIdQuery query, CancellationToken cancellationToken = default) =>
        await staffRepository.FindByIdAsync(query.Id, cancellationToken);
}
