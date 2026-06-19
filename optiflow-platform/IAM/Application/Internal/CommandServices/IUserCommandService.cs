using System.Threading;
using System.Threading.Tasks;
using optiflow_platform.IAM.Domain.Model.Aggregates;
using optiflow_platform.IAM.Domain.Model.Commands;
using optiflow_platform.IAM.Domain.Model.Queries;
using optiflow_platform.Shared.Application.Patterns;
using optiflow_platform.Shared.Application.Model;

namespace optiflow_platform.IAM.Application.CommandServices;

public interface IUserCommandService
{
    Task<Result<AuthenticatedUser>> Handle(SignInCommand command, CancellationToken cancellationToken);
    Task<Result> Handle(SignUpCommand command, CancellationToken cancellationToken);
    Task<Result<AuthenticatedUser>> Handle(UpdateUserEmailCommand command, CancellationToken cancellationToken);
    Task<Result<User>> Handle(UpdateUserPasswordCommand command, CancellationToken cancellationToken);
    Task<Result<AuthenticatedUser>> Handle(GoogleSignInCommand command, CancellationToken cancellationToken);
}
