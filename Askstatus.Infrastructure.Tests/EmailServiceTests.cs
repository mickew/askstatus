﻿using Askstatus.Application.Interfaces;
using Askstatus.Common.Models;
using Askstatus.Domain;
using Askstatus.Domain.Constants;
using Askstatus.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Askstatus.Infrastructure.Tests;
public class EmailServiceTests
{
    [Fact]
    public async Task SendEmailAsync_ShouldSendEmail()
    {
        // Arrange
        var mailMessage = new MailMessage("info@askstatus.com", "Anders.Anderson@test.com", "andersa", "Anders", "Test Subject", MailMessageBody.MailBodyTest());
        var mailSettings = new MailSettings { Enabled = true, Port = 0, Host = "host", EnableSsl = false };
        var optionsSnapshot = new Mock<IOptionsSnapshot<MailSettings>>();
        optionsSnapshot.Setup(x => x.Value).Returns(mailSettings);
        var logger = new Mock<ILogger<EmailService>>();
        var smtpClientMock = new Mock<IAskStatusSmtpClient>();
        smtpClientMock.Setup(x => x.SendEmailAsync(mailMessage)).ReturnsAsync(true);

        var emailService = new EmailService(logger.Object, smtpClientMock.Object, optionsSnapshot.Object);

        // Act
        var result = await emailService.SendEmailAsync(mailMessage);

        // Assert
        result.Should().BeTrue();
        smtpClientMock.Verify(x => x.SendEmailAsync(mailMessage), Times.Once);
    }

    [Fact]
    public async Task SendEmailAsync_ShouldNotSendEmail()
    {
        // Arrange
        var mailMessage = new MailMessage("info@askstatus.com", "Anders.Anderson@test.com", "andersa", "Anders", "Test Subject", MailMessageBody.MailBodyTest());
        var mailSettings = new MailSettings { Enabled = false, Port = 0, Host = "host", EnableSsl = false };
        var optionsSnapshot = new Mock<IOptionsSnapshot<MailSettings>>();
        optionsSnapshot.Setup(x => x.Value).Returns(mailSettings);
        var logger = new Mock<ILogger<EmailService>>();
        var smtpClientMock = new Mock<IAskStatusSmtpClient>();
        smtpClientMock.Setup(x => x.SendEmailAsync(mailMessage)).ReturnsAsync(false);

        var emailService = new EmailService(logger.Object, smtpClientMock.Object, optionsSnapshot.Object);

        // Act
        var result = await emailService.SendEmailAsync(mailMessage);

        // Assert
        result.Should().BeFalse();
        smtpClientMock.Verify(x => x.SendEmailAsync(mailMessage), Times.Never);
    }
}
