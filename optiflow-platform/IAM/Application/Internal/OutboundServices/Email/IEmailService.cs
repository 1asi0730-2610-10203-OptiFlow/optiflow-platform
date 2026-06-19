using System.Threading.Tasks;

namespace atelier_platform_aplicaciones_web.IAM.Application.Internal.OutboundServices.Email;

public interface IEmailService
{
    Task SendPasswordRecoveryEmailAsync(string to, string token);
}
