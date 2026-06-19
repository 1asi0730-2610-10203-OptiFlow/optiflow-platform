using System.Threading.Tasks;

namespace optiflow_platform.IAM.Application.Internal.OutboundServices.Email;

public interface IEmailService
{
    Task SendPasswordRecoveryEmailAsync(string to, string token);
}
