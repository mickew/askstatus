using Askstatus.Common.Models;
using Askstatus.Domain.Constants;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Utils;

namespace EmailFormatTestApp;

internal class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        var mailMessagePasswordReset = new MailMessage("info@askstatus.com", "Anders.Anderson@test.com", "andersa", "Anders", "Askstatus reset password request",
            MailMessageBody.ResetPasswordMailBody("https://localhost/api/ResetPassword", "Anders"));
        var mailMessageRegistrationConfirm = new MailMessage("info@askstatus.com", "Anders.Anderson@test.com", "andersa", "Anders", "Welcome to Askstatus, Anders",
            MailMessageBody.RegistrationConfirmationMailBody("andersa", "https://localhost/api/ConfirmRegistration", "Anders"));

        using (var client = new SmtpClient())
        {
            client.Connect("localhost", 1025, false);

            var message = await CreateMessageAsync(mailMessageRegistrationConfirm);
            await client.SendAsync(message);

            message = await CreateMessageAsync(mailMessagePasswordReset);
            await client.SendAsync(message);

            client.Disconnect(true);
        }
    }

    private static async Task<MimeMessage> CreateMessageAsync(MailMessage mailMessage)
    {
        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(mailMessage.From));
        message.To.Add(MailboxAddress.Parse(mailMessage.To));
        message.Subject = mailMessage.Subject;
        var builder = new BodyBuilder();
        builder.TextBody = mailMessage.Body.Text;

        var image = await builder.LinkedResources.AddAsync("images/logo-32x32.png");
        image.ContentId = MimeUtils.GenerateMessageId();

        builder.HtmlBody = mailMessage.Body.Html.Replace("{imageCid}", image.ContentId);
        message.Body = builder.ToMessageBody();
        return message;
    }
}
