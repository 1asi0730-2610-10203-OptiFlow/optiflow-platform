using System.Threading;
using System.Threading.Tasks;
using optiflow_platform.IAM.Domain.Model.Commands;
using optiflow_platform.Shared.Application.Patterns;

namespace optiflow_platform.IAM.Application.CommandServices;

public interface IPasswordRecoveryCommandService
{
    Task<Result> Handle(GeneratePasswordRecoveryTokenCommand command, CancellationToken cancellationToken);
    Task<Result> Handle(ResetPasswordCommand command, CancellationToken cancellationToken);
}
