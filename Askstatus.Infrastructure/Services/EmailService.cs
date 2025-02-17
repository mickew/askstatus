using Askstatus.Application.Interfaces;
using Askstatus.Common.Models;
using Askstatus.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Askstatus.Infrastructure.Services;
public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly IAskStatusSmtpClient _smtpClient;
    private readonly IOptions<MailSettings> _options;

    public EmailService(ILogger<EmailService> logger, IAskStatusSmtpClient smtpClient, IOptions<MailSettings> options)
    {
        _logger = logger;
        _smtpClient = smtpClient;
        _options = options;
    }

    public async Task<bool> SendEmailAsync(MailMessage mailMessage)
    {
        if (!_options.Value.Enabled)
        {
            _logger.LogInformation("Mail settings is not enabled.");
            return false;
        }
        return await _smtpClient.SendEmailAsync(mailMessage);
    }
}
