using Askstatus.Common.Models;
using Askstatus.Domain;
using Askstatus.Domain.Constants;
using Askstatus.Infrastructure.Mail;
using Askstatus.Infrastructure.Tests.Common;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

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
        var mailMessage = new MailMessage("info@askstatus.com", "Anders.Anderson@test.com", "andersa", "Anders", "Askstatus reset password request", MailMessageBody.ResetPasswordMailBody("https://localhost/api/ResetPassword", "Anders"));
        var mailSettings = new MailSettings { Port = port, Host = host, EnableSsl = false };
        var optionsSnapshot = new Mock<IOptionsSnapshot<MailSettings>>();
        optionsSnapshot.Setup(x => x.Value).Returns(mailSettings);
        var logger = new Mock<ILogger<MailKitSmtpClient>>();
        var smtpClient = new MailKitSmtpClient(logger.Object, optionsSnapshot.Object);

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
        var optionsSnapshot = new Mock<IOptionsSnapshot<MailSettings>>();
        optionsSnapshot.Setup(x => x.Value).Returns(mailSettings);
        var logger = new Mock<ILogger<MailKitSmtpClient>>();
        var smtpClient = new MailKitSmtpClient(logger.Object, optionsSnapshot.Object);

        // Act
        var result = await smtpClient.SendEmailAsync(mailMessage);

        // Assert
        result.Should().BeTrue();
    }

}
