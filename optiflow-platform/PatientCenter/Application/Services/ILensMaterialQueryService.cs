using optiflow_platform.PatientCenter.Domain.Model.Entities;
using optiflow_platform.PatientCenter.Domain.Model.Queries;

namespace optiflow_platform.PatientCenter.Application.Services;

public interface ILensMaterialQueryService
{
    Task<IEnumerable<LensMaterial>> Handle(GetAllLensMaterialsQuery query,
        CancellationToken cancellationToken = default);
}