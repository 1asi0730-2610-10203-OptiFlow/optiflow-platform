using System;
using System.Threading;
using System.Threading.Tasks;
using optiflow_platform.IAM.Application.ACL;
using optiflow_platform.IAM.Application.Internal.OutboundServices.Hashing;
using optiflow_platform.IAM.Domain.Model.Aggregates;
using optiflow_platform.IAM.Domain.Model.ValueObjects;
using optiflow_platform.IAM.Domain.Repositories;
using optiflow_platform.Shared.Domain.Repositories;

namespace optiflow_platform.IAM.Application.Internal.ACL;

/// <inheritdoc />
public class ClientAccountService(
    IUserRepository userRepository,
    IHashingService hashingService,
    IUnitOfWork unitOfWork) : IClientAccountService
{
    public async Task EnsureClientUserAsync(string email, Guid opticAccountId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email)) return;

        // Normalize so a client is one login identity regardless of casing/whitespace. Without this,
        // "Sarah@x.com" and "sarah@x.com" look like different users, so the existing account is missed
        // and a duplicate is created — which detaches the patient from their orders. Reuse, never recreate.
        var emailAddress = new EmailAddress(email.Trim().ToLowerInvariant());
        var existing = await userRepository.FindByEmailAsync(emailAddress, cancellationToken);
        if (existing != null)
        {
            // A client account already exists for this email — link it to this optic if it isn't
            // attached to one yet, so the added patient and the existing client are connected now
            // (instead of only on the client's next sign-in). Owners are never reassigned.
            if (existing.Role == UserRole.Client && existing.AccountId == null)
            {
                existing.AssignAccount(opticAccountId);
                userRepository.Update(existing);
                await unitOfWork.CompleteAsync();
            }
            return;
        }

        // Passwordless clients still need a stored hash; use an unguessable random one they never use.
        var placeholderPassword = hashingService.Encode(Guid.NewGuid().ToString("N"));
        var user = new User(emailAddress, new Password(placeholderPassword));
        user.AssignRole(UserRole.Client);
        user.AssignAccount(opticAccountId);

        await userRepository.AddAsync(user);
        await unitOfWork.CompleteAsync();
    }
}
