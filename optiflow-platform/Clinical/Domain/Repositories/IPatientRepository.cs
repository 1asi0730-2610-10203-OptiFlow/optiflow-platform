using optiflow_platform.Clinical.Domain.Model.Aggregates;
using optiflow_platform.Shared.Domain.Repositories;

namespace optiflow_platform.Clinical.Domain.Repositories;

/// <summary>Repository contract for <see cref="Patient"/> aggregates.</summary>
public interface IPatientRepository : IBaseRepository<Patient>
{
    /// <summary>Finds a patient by their DNI number. Returns null if not found.</summary>
    Task<Patient?> FindByDniAsync(string dni, CancellationToken cancellationToken = default);
}
