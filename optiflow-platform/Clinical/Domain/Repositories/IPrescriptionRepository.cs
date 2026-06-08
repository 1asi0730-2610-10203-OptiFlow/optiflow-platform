using optiflow_platform.Clinical.Domain.Model.Entities;
using optiflow_platform.Shared.Domain.Repositories;

namespace optiflow_platform.Clinical.Domain.Repositories;

/// <summary>Repository contract for <see cref="Prescription"/> entities.</summary>
public interface IPrescriptionRepository : IBaseRepository<Prescription>
{
    /// <summary>Returns all prescriptions that belong to a given clinical record.</summary>
    Task<IEnumerable<Prescription>> FindByClinicalRecordIdAsync(int clinicalRecordId, CancellationToken cancellationToken = default);
}
