using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Repositories;
using optiflow_platform.Staff.Domain.Model.Aggregates;
using optiflow_platform.Staff.Domain.Repositories;

namespace optiflow_platform.Staff.Infrastructure.Persistence.EFC.Repositories;

/// <summary>EF Core implementation of <see cref="IStaffRepository"/>.</summary>
public class StaffRepository(AppDbContext context)
    : BaseRepository<StaffMember>(context), IStaffRepository;
