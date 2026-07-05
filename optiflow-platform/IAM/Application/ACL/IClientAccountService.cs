using System;
using System.Threading;
using System.Threading.Tasks;

namespace optiflow_platform.IAM.Application.ACL;

/// <summary>
///     Inbound port other contexts use to turn a patient into a valid client user of the optic that
///     created them, so the patient can log in (passwordless) and see the orders that optic owns.
/// </summary>
public interface IClientAccountService
{
    /// <summary>
    ///     Ensures a CLIENT user exists for the given email, owned by the given optic account. No-op if
    ///     a user with that email already exists.
    /// </summary>
    Task EnsureClientUserAsync(string email, Guid opticAccountId, CancellationToken cancellationToken = default);
}
