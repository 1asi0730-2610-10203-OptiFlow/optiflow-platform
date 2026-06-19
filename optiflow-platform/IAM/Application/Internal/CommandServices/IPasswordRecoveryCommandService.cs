using System.Threading;
using System.Threading.Tasks;
using atelier_platform_aplicaciones_web.IAM.Domain.Model.Commands;
using atelier_platform_aplicaciones_web.Shared.Application.Model;

namespace atelier_platform_aplicaciones_web.IAM.Application.CommandServices;

public interface IPasswordRecoveryCommandService
{
    Task<Result> Handle(GeneratePasswordRecoveryTokenCommand command, CancellationToken cancellationToken);
    Task<Result> Handle(ResetPasswordCommand command, CancellationToken cancellationToken);
}
