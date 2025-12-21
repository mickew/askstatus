using Askstatus.Application.Interfaces;
using Askstatus.Common.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Askstatus.Domain;
using Askstatus.Application.System;

namespace Askstatus.Application.Tests;

public class SendEmailCommandHandlerTests
{
    private readonly Mock<IEmailService> _emailServiceMock = new();
    private readonly Mock<ILogger<SendEmailCommandHandler>> _loggerMock = new();
    private readonly Mock<IOptionsSnapshot<MailSettings>> _optionsMock = new();
    private readonly MailSettings _mailSettings = new() { Account = "test@domain.com" };

    public SendEmailCommandHandlerTests()
    {
        _optionsMock.Setup(x => x.Value).Returns(_mailSettings);
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenEmailIsSentSuccessfully()
    {
        // Arrange
        _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<MailMessage>())).ReturnsAsync(true);
        var handler = new SendEmailCommandHandler(_emailServiceMock.Object, _loggerMock.Object, _optionsMock.Object);
        var command = new SendEmailCommand("recipient@domain.com", "header", "first", "subject", "body");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenEmailSendingFails()
    {
        // Arrange
        _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<MailMessage>())).ReturnsAsync(false);
        var handler = new SendEmailCommandHandler(_emailServiceMock.Object, _loggerMock.Object, _optionsMock.Object);
        var command = new SendEmailCommand("recipient@domain.com", "header", "first", "subject", "body");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "Failed to send email");
    }
}
