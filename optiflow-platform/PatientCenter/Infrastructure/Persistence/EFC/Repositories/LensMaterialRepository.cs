using optiflow_platform.PatientCenter.Domain.Model.Entities;
using optiflow_platform.PatientCenter.Domain.Repositories;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using optiflow_platform.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace optiflow_platform.PatientCenter.Infrastructure.Persistence.EFC.Repositories;

public class LensMaterialRepository(AppDbContext context)
    : BaseRepository<LensMaterial>(context), ILensMaterialRepository
{
}