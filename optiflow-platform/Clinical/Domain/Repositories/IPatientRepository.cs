using optiflow_platform.Clinical.Domain.Model.Aggregates;
using optiflow_platform.Shared.Domain.Repositories;

namespace optiflow_platform.Clinical.Domain.Repositories;

/// <summary>Repository contract for <see cref="Patient"/> aggregates.</summary>
public interface IPatientRepository : IBaseRepository<Patient>
{
    /// <summary>Finds a patient by their DNI number. Returns null if not found.</summary>
    Task<Patient?> FindByDniAsync(string dni, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Finds a patient by email ignoring the current account scope, so the patient portal
    ///     can resolve the record for a patient user who does not belong to any account.
    /// </summary>
    Task<Patient?> FindByEmailIgnoringScopeAsync(string email, CancellationToken cancellationToken = default);
}
