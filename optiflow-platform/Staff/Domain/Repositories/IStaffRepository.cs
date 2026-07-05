using optiflow_platform.Shared.Domain.Repositories;
using optiflow_platform.Staff.Domain.Model.Aggregates;

namespace optiflow_platform.Staff.Domain.Repositories;

/// <summary>Repository contract for <see cref="StaffMember"/> aggregates.</summary>
public interface IStaffRepository : IBaseRepository<StaffMember>;
