using System;
using System.Threading;
using System.Threading.Tasks;
using optiflow_platform.IAM.Application.CommandServices;
using optiflow_platform.IAM.Application.Internal.OutboundServices.Hashing;
using optiflow_platform.IAM.Application.Internal.OutboundServices.Patients;
using optiflow_platform.IAM.Application.Internal.OutboundServices.Tokens;
using optiflow_platform.IAM.Domain.Model;
using optiflow_platform.IAM.Domain.Model.Aggregates;
using optiflow_platform.IAM.Domain.Model.Commands;
using optiflow_platform.IAM.Domain.Model.Queries;
using optiflow_platform.IAM.Domain.Model.ValueObjects;
using optiflow_platform.IAM.Domain.Repositories;
using optiflow_platform.Shared.Application.Patterns;
using optiflow_platform.Shared.Application.Model;
using optiflow_platform.Shared.Domain.Repositories;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;

namespace optiflow_platform.IAM.Application.Internal.CommandServices;

public class UserCommandService(
    IUserRepository userRepository,
    IHashingService hashingService,
    ITokenService tokenService,
    IUnitOfWork unitOfWork,
    IPatientDirectoryService patientDirectory,
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

        // A client may sign up before their optic has created the matching patient record; link
        // them to their optic on first sign-in where the match now exists.
        await TryLinkClientToOpticAsync(user, cancellationToken);

        var token = tokenService.GenerateToken(user.Email.Value);
        return Result<AuthenticatedUser>.Success(new AuthenticatedUser(user, token));
    }

    private async Task TryLinkClientToOpticAsync(User user, CancellationToken cancellationToken)
    {
        // Auto-match strategy: any account-less user whose email matches a patient an optic created
        // is a client of that optic. Owners (AccountId already set) are never reclassified.
        if (user.AccountId != null) return;

        var opticAccountId = await patientDirectory.FindOpticAccountIdByEmailAsync(user.Email.Value, cancellationToken);
        if (opticAccountId is not { } accountId) return;

        user.AssignRole(UserRole.Client);
        user.AssignAccount(accountId);
        userRepository.Update(user);
        await unitOfWork.CompleteAsync();
    }

    public async Task<Result<AuthenticatedUser>> Handle(SignUpCommand command, CancellationToken cancellationToken)
    {
        if (await userRepository.ExistsByEmailAsync(command.Email, cancellationToken))
        {
            return Result<AuthenticatedUser>.Failure(IamError.EmailAlreadyInUse, "iam.error.email.alreadyInUse");
        }

        var passwordHash = hashingService.Encode(command.Password.Value);
        var user = new User(command.Email, new Password(passwordHash));
        user.AssignRole(command.Role);

        // Tie a client to the optic that already registered them as a patient (matched by email),
        // so everything they do is owned by that optic.
        if (command.Role == UserRole.Client)
        {
            var opticAccountId = await patientDirectory.FindOpticAccountIdByEmailAsync(command.Email.Value, cancellationToken);
            if (opticAccountId is { } accountId)
                user.AssignAccount(accountId);
        }

        await userRepository.AddAsync(user);
        await unitOfWork.CompleteAsync();

        var token = tokenService.GenerateToken(user.Email.Value);
        return Result<AuthenticatedUser>.Success(new AuthenticatedUser(user, token));
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
                var randomPassword = Guid.NewGuid().ToString("N");
                var passwordHash = hashingService.Encode(randomPassword);
                user = new User(new EmailAddress(email), new Password(passwordHash), new GoogleId(googleId));
                await userRepository.AddAsync(user);
                await unitOfWork.CompleteAsync();
            }
            else
            {
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
