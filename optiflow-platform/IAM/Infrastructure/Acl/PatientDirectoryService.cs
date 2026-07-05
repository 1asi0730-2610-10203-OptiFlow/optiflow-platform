using System;
using System.Threading;
using System.Threading.Tasks;
using optiflow_platform.Clinical.Domain.Repositories;
using optiflow_platform.IAM.Application.Internal.OutboundServices.Patients;

namespace optiflow_platform.IAM.Infrastructure.Acl;

/// <summary>
///     Anti-corruption implementation of <see cref="IPatientDirectoryService"/> backed by the
///     Clinical patient repository's scope-ignoring email lookup.
/// </summary>
public class PatientDirectoryService(IPatientRepository patientRepository) : IPatientDirectoryService
{
    public async Task<Guid?> FindOpticAccountIdByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email)) return null;
        var patient = await patientRepository.FindByEmailIgnoringScopeAsync(email, cancellationToken);
        return patient?.AccountId;
    }
}
