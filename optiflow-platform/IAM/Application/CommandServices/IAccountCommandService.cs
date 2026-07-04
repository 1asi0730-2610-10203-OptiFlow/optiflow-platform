using System.Threading;
using System.Threading.Tasks;
using optiflow_platform.IAM.Domain.Model.Aggregates;
using optiflow_platform.IAM.Domain.Model.Commands;
using optiflow_platform.Shared.Application.Model;

namespace optiflow_platform.IAM.Application.CommandServices;

public interface IAccountCommandService
{
    Task<Result<Account>> Handle(CreateAccountCommand command, CancellationToken cancellationToken);
}
