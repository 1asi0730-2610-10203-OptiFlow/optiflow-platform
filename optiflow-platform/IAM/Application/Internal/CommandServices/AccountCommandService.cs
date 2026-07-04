using System.Threading;
using System.Threading.Tasks;
using optiflow_platform.IAM.Application.CommandServices;
using optiflow_platform.IAM.Domain.Model;
using optiflow_platform.IAM.Domain.Model.Aggregates;
using optiflow_platform.IAM.Domain.Model.Commands;
using optiflow_platform.IAM.Domain.Repositories;
using optiflow_platform.Shared.Application.Model;
using optiflow_platform.Shared.Domain.Repositories;

namespace optiflow_platform.IAM.Application.Internal.CommandServices;

public class AccountCommandService(
    IAccountRepository accountRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork) : IAccountCommandService
{
    public async Task<Result<Account>> Handle(CreateAccountCommand command, CancellationToken cancellationToken)
    {
        var user = await userRepository.FindByIdAsync(command.OwnerUserId, cancellationToken);
        if (user == null)
            return Result<Account>.Failure(IamError.UserNotFound, "iam.error.user.notFound");

        if (user.AccountId != null)
            return Result<Account>.Failure(IamError.AccountAlreadyOnboarded, "iam.error.account.alreadyOnboarded");

        var account = new Account(command);
        await accountRepository.AddAsync(account, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);

        user.AssignAccount(account.Id);
        userRepository.Update(user);
        await unitOfWork.CompleteAsync(cancellationToken);

        return Result<Account>.Success(account);
    }
}
