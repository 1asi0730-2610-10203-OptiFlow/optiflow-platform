using optiflow_platform.PatientCenter.Application.Services;
using optiflow_platform.PatientCenter.Domain.Model.Entities;
using optiflow_platform.PatientCenter.Domain.Model.Queries;
using optiflow_platform.PatientCenter.Domain.Repositories;

namespace optiflow_platform.PatientCenter.Application.Internal.QueryServices;

public class LensMaterialQueryService(ILensMaterialRepository lensMaterialRepository)
    : ILensMaterialQueryService
{
    public async Task<IEnumerable<LensMaterial>> Handle(GetAllLensMaterialsQuery query,
        CancellationToken cancellationToken = default) =>
        await lensMaterialRepository.ListAsync(cancellationToken);
}