using Askstatus.Common.Models;

namespace Askstatus.Application.Interfaces;
public interface IAskStatusSmtpClient
{
    Task<bool> SendEmailAsync(MailMessage mailMessage);
}
