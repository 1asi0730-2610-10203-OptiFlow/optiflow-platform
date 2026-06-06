using optiflow_platform.Clinical.Domain.Model.Entities;
using optiflow_platform.Shared.Domain.Repositories;

namespace optiflow_platform.Clinical.Domain.Repositories;

/// <summary>Repository contract for <see cref="ClinicalRecord"/> entities.</summary>
public interface IClinicalRecordRepository : IBaseRepository<ClinicalRecord>
{
    /// <summary>Returns the clinical record linked to the given patient. Returns null if not found.</summary>
    Task<ClinicalRecord?> FindByPatientIdAsync(int patientId, CancellationToken cancellationToken = default);
}
