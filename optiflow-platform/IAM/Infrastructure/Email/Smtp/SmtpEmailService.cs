using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using atelier_platform_aplicaciones_web.IAM.Application.Internal.OutboundServices.Email;
using atelier_platform_aplicaciones_web.IAM.Resources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace atelier_platform_aplicaciones_web.IAM.Infrastructure.Email.Smtp;

public class SmtpEmailService(ILogger<SmtpEmailService> logger, IConfiguration configuration, IStringLocalizer<IamMessages> localizer) : IEmailService
{
    public async Task SendPasswordRecoveryEmailAsync(string to, string token)
    {
        var host = configuration["Smtp:Host"];
        var portStr = configuration["Smtp:Port"];
        var username = configuration["Smtp:Username"];
        var password = configuration["Smtp:Password"];
        var frontendUrl = configuration["AppSettings:FrontendUrl"];

        if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || !int.TryParse(portStr, out var port))
        {
            logger.LogWarning("SMTP Settings are not fully configured. Simulating email to {Email}. Token: {Token}", to, token);
            return;
        }

        var subject = localizer["email.recovery.subject"].Value;
        var bodyText = localizer["email.recovery.body", frontendUrl ?? "", token].Value;

        using var client = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(username, password),
            EnableSsl = true
        };

        using var mailMessage = new MailMessage
        {
            From = new MailAddress(username, "Atelier Platform"),
            Subject = subject,
            Body = bodyText,
            IsBodyHtml = false,
        };
        mailMessage.To.Add(to);

        try
        {
            await client.SendMailAsync(mailMessage);
            logger.LogInformation("Password recovery email successfully sent to {Email}", to);
        }
        catch (SmtpException ex)
        {
            logger.LogError(ex, "Failed to send password recovery email to {Email}", to);
            throw;
        }
    }
}
