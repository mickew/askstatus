using Askstatus.Common.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Testcontainers.Xunit;
using Testcontainers.Papercut;
using Askstatus.Domain.Constants;
using Docker.DotNet.Models;
using Askstatus.Infrastructure.Mail;
using Askstatus.Infrastructure.Tests.Common;

namespace Askstatus.Infrastructure.Tests;

public class PapercutMailKitSmtpClientTests : IClassFixture<SMTPServerFixture>
{
    private readonly SMTPServerFixture _sMTPServerFixture;

    public PapercutMailKitSmtpClientTests(SMTPServerFixture sMTPServerFixture)
    {
        _sMTPServerFixture = sMTPServerFixture;
    }

    [Fact]
    public async Task SendEmailAsync_WithResetPassword_ReturnsTrue()
    {
        var port = _sMTPServerFixture.PapercutContainer.GetMappedPublicPort(25);
        var host = _sMTPServerFixture.PapercutContainer.Hostname;
        //var port = 1025;
        //var host = "localhost";

        // Arrange
        var mailMessage = new MailMessage("info@askstatus.com", "Anders.Anderson@test.com", "andersa", "Anders" ,"Askstatus reset password request" , MailMessageBody.ResetPasswordMailBody("https://localhost/api/ResetPassword", "Anders"));
        var mailSettings = new MailSettings { Port = port, Host = host, EnableSsl = false };
        IOptions<MailSettings> options = Options.Create(mailSettings);
        var logger = new Mock<ILogger<MailKitSmtpClient>>();
        var smtpClient = new MailKitSmtpClient(logger.Object, options);

        // Act
        var result = await smtpClient.SendEmailAsync(mailMessage);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task SendEmailAsync_WithRegistrationConfirm_ReturnsTrue()
    {
        var port = _sMTPServerFixture.PapercutContainer.GetMappedPublicPort(25);
        var host = _sMTPServerFixture.PapercutContainer.Hostname;
        //var port = 1025;
        //var host = "localhost";

        // Arrange
        var mailMessage = new MailMessage("info@askstatus.com", "Anders.Anderson@test.com", "andersa", "Anders", "Welcome to Askstatus, Anders", MailMessageBody.RegistrationConfirmationMailBody("andersa", "https://localhost/api/ResetPassword", "Anders"));
        var mailSettings = new MailSettings { Port = port, Host = host, EnableSsl = false };
        IOptions<MailSettings> options = Options.Create(mailSettings);
        var logger = new Mock<ILogger<MailKitSmtpClient>>();
        var smtpClient = new MailKitSmtpClient(logger.Object, options);

        // Act
        var result = await smtpClient.SendEmailAsync(mailMessage);

        // Assert
        result.Should().BeTrue();
    }

}
