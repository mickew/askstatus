using Askstatus.Common.Models;

namespace Askstatus.Application.Interfaces;
public interface IEmailService
{
    Task<bool> SendEmailAsync(MailMessage mailMessage);
}
