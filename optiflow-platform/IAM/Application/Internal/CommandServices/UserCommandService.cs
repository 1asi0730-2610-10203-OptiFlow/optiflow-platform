using System;
using System.Threading;
using System.Threading.Tasks;
using atelier_platform_aplicaciones_web.IAM.Application.CommandServices;
using atelier_platform_aplicaciones_web.IAM.Application.Internal.OutboundServices.Hashing;
using atelier_platform_aplicaciones_web.IAM.Application.Internal.OutboundServices.Tokens;
using atelier_platform_aplicaciones_web.IAM.Domain.Model;
using atelier_platform_aplicaciones_web.IAM.Domain.Model.Aggregates;
using atelier_platform_aplicaciones_web.IAM.Domain.Model.Commands;
using atelier_platform_aplicaciones_web.IAM.Domain.Model.Queries;
using atelier_platform_aplicaciones_web.IAM.Domain.Model.ValueObjects;
using atelier_platform_aplicaciones_web.IAM.Domain.Repositories;
using atelier_platform_aplicaciones_web.Shared.Application.Model;
using atelier_platform_aplicaciones_web.Shared.Domain.Repositories;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;

namespace atelier_platform_aplicaciones_web.IAM.Application.Internal.CommandServices;

public class UserCommandService(
    IUserRepository userRepository,
    IHashingService hashingService,
    ITokenService tokenService,
    IUnitOfWork unitOfWork,
    IConfiguration configuration) : IUserCommandService
{
    public async Task<Result<AuthenticatedUser>> Handle(SignInCommand command, CancellationToken cancellationToken)
    {
        var user = await userRepository.FindByEmailAsync(command.Email, cancellationToken);
        if (user == null || !hashingService.Matches(command.Password.Value, user.Password.Value))
        {
            return Result<AuthenticatedUser>.Failure(IamError.InvalidCredentials, "iam.error.credentials.invalid");
        }

        if (user.Status == UserStatus.Inactive)
        {
            return Result<AuthenticatedUser>.Failure(IamError.InvalidCredentials, "iam.error.credentials.invalid");
        }

        var token = tokenService.GenerateToken(user.Email.Value);
        return Result<AuthenticatedUser>.Success(new AuthenticatedUser(user, token));
    }

    public async Task<Result> Handle(SignUpCommand command, CancellationToken cancellationToken)
    {
        if (await userRepository.ExistsByEmailAsync(command.Email, cancellationToken))
        {
            return Result.Failure(IamError.EmailAlreadyInUse, "iam.error.email.alreadyInUse");
        }

        var passwordHash = hashingService.Encode(command.Password.Value);
        var user = new User(command.Email, new Password(passwordHash));
        
        await userRepository.AddAsync(user);
        await unitOfWork.CompleteAsync();
        
        return Result.Success();
    }

    public async Task<Result<AuthenticatedUser>> Handle(UpdateUserEmailCommand command, CancellationToken cancellationToken)
    {
        var user = await userRepository.FindByIdAsync(command.UserId.Value, cancellationToken);
        if (user == null)
            return Result<AuthenticatedUser>.Failure(IamError.UserNotFound, "iam.error.user.notFound");

        if (user.Email.Value != command.NewEmail.Value && await userRepository.ExistsByEmailAsync(command.NewEmail, cancellationToken))
            return Result<AuthenticatedUser>.Failure(IamError.EmailAlreadyInUse, "iam.error.email.alreadyInUse");

        user.ChangeEmail(command.NewEmail);
        userRepository.Update(user);
        await unitOfWork.CompleteAsync();

        var token = tokenService.GenerateToken(user.Email.Value);
        return Result<AuthenticatedUser>.Success(new AuthenticatedUser(user, token));
    }

    public async Task<Result<User>> Handle(UpdateUserPasswordCommand command, CancellationToken cancellationToken)
    {
        var user = await userRepository.FindByIdAsync(command.UserId.Value, cancellationToken);
        if (user == null)
            return Result<User>.Failure(IamError.UserNotFound, "iam.error.user.notFound");

        if (!hashingService.Matches(command.CurrentPassword.Value, user.Password.Value))
        {
            return Result<User>.Failure(IamError.InvalidCurrentPassword, "iam.error.currentPassword.invalid");
        }

        user.ChangePassword(new Password(hashingService.Encode(command.NewPassword.Value)));
        userRepository.Update(user);
        await unitOfWork.CompleteAsync();

        return Result<User>.Success(user);
    }

    public async Task<Result<AuthenticatedUser>> Handle(GoogleSignInCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var googleClientId = configuration["Google:ClientId"];
            if (string.IsNullOrEmpty(googleClientId))
            {
                return Result<AuthenticatedUser>.Failure(IamError.InternalServerError, "iam.error.googleToken.invalid");
            }

            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new[] { googleClientId }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(command.IdToken, settings);
            
            var googleId = payload.Subject;
            var email = payload.Email;

            var user = await userRepository.FindByEmailAsync(new EmailAddress(email), cancellationToken);
            if (user == null)
            {
                // Register new user with a secure random password
                var randomPassword = Guid.NewGuid().ToString("N");
                var passwordHash = hashingService.Encode(randomPassword);
                user = new User(new EmailAddress(email), new Password(passwordHash), new GoogleId(googleId));
                await userRepository.AddAsync(user);
                await unitOfWork.CompleteAsync();
            }
            else
            {
                // If user exists but doesn't have a googleId linked, link it
                if (user.GoogleId == null || string.IsNullOrEmpty(user.GoogleId.Value))
                {
                    user.LinkGoogleAccount(new GoogleId(googleId));
                    userRepository.Update(user);
                    await unitOfWork.CompleteAsync();
                }
                else if (user.GoogleId.Value != googleId)
                {
                    return Result<AuthenticatedUser>.Failure(IamError.InvalidGoogleToken, "iam.error.googleToken.invalid");
                }
            }

            if (user.Status == UserStatus.Inactive)
            {
                return Result<AuthenticatedUser>.Failure(IamError.InvalidCredentials, "iam.error.credentials.invalid");
            }

            var token = tokenService.GenerateToken(user.Email.Value);
            return Result<AuthenticatedUser>.Success(new AuthenticatedUser(user, token));
        }
        catch (InvalidJwtException)
        {
            return Result<AuthenticatedUser>.Failure(IamError.InvalidGoogleToken, "iam.error.googleToken.invalid");
        }
        catch (Exception ex)
        {
            return Result<AuthenticatedUser>.Failure(IamError.InternalServerError, ex.Message);
        }
    }
}
