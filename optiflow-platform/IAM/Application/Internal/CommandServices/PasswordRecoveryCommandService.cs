using System;
using System.Threading;
using System.Threading.Tasks;
using optiflow_platform.IAM.Application.CommandServices;
using optiflow_platform.IAM.Application.Internal.OutboundServices.Email;
using optiflow_platform.IAM.Application.Internal.OutboundServices.Hashing;
using optiflow_platform.IAM.Domain.Model;
using optiflow_platform.IAM.Domain.Model.Commands;
using optiflow_platform.IAM.Domain.Model.Entities;
using optiflow_platform.IAM.Domain.Model.ValueObjects;
using optiflow_platform.IAM.Domain.Repositories;
using optiflow_platform.Shared.Application.Model;
using optiflow_platform.Shared.Domain.Repositories;

namespace optiflow_platform.IAM.Application.Internal.CommandServices;

public class PasswordRecoveryCommandService(
    IUserRepository userRepository,
    IPasswordRecoveryTokenRepository tokenRepository,
    IEmailService emailService,
    IHashingService hashingService,
    IUnitOfWork unitOfWork) : IPasswordRecoveryCommandService
{
    public async Task<Result> Handle(GeneratePasswordRecoveryTokenCommand command, CancellationToken cancellationToken)
    {
        var user = await userRepository.FindByEmailAsync(command.Email, cancellationToken);
        if (user == null)
            return Result.Failure(IamError.UserNotFound, "iam.error.user.notFound");

        var tokenString = Guid.NewGuid().ToString();
        var token = new PasswordRecoveryToken(tokenString, user.Id.Value, 60);
        
        await tokenRepository.AddAsync(token);
        await unitOfWork.CompleteAsync();

        await emailService.SendPasswordRecoveryEmailAsync(user.Email.Value, tokenString);

        return Result.Success();
    }

    public async Task<Result> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        var token = await tokenRepository.FindByTokenHashAsync(command.Token, cancellationToken);
        
        if (token == null)
            return Result.Failure(IamError.InvalidOrExpiredToken, "iam.error.token.invalidOrExpired");

        if (!token.IsValid())
            return Result.Failure(IamError.ExpiredOrUsedToken, "iam.error.token.expiredOrUsed");

        var user = await userRepository.FindUserByIdAsync(new UserId(token.UserId), cancellationToken);
        if (user == null)
            return Result.Failure(IamError.UserNotFound, "iam.error.user.notFound");

        user.ChangePassword(new Password(hashingService.Encode(command.NewPassword.Value)));
        token.MarkAsUsed();

        userRepository.Update(user);
        tokenRepository.Update(token);
        
        await unitOfWork.CompleteAsync();

        return Result.Success();
    }
}
