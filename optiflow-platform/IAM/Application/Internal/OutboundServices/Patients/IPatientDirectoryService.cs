using System;
using System.Threading;
using System.Threading.Tasks;

namespace optiflow_platform.IAM.Application.Internal.OutboundServices.Patients;

/// <summary>
///     Outbound port that lets IAM discover which optic (account) a client belongs to, by matching
///     the client's email against the patient records the optics have created. Implemented over the
///     Clinical context so IAM stays free of a direct dependency on that context's persistence.
/// </summary>
public interface IPatientDirectoryService
{
    /// <summary>
    ///     Returns the account id of the optic that owns a patient record with the given email,
    ///     or null if no patient matches. The lookup crosses account boundaries on purpose.
    /// </summary>
    Task<Guid?> FindOpticAccountIdByEmailAsync(string email, CancellationToken cancellationToken = default);
}
