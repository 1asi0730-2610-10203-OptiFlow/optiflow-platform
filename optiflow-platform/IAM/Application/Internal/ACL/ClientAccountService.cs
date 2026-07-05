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

        var emailAddress = new EmailAddress(email);
        if (await userRepository.ExistsByEmailAsync(emailAddress, cancellationToken)) return;

        // Passwordless clients still need a stored hash; use an unguessable random one they never use.
        var placeholderPassword = hashingService.Encode(Guid.NewGuid().ToString("N"));
        var user = new User(emailAddress, new Password(placeholderPassword));
        user.AssignRole(UserRole.Client);
        user.AssignAccount(opticAccountId);

        await userRepository.AddAsync(user);
        await unitOfWork.CompleteAsync();
    }
}
