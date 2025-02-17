using Askstatus.Application.Errors;
using Askstatus.Application.Interfaces;
using Askstatus.Application.System;
using Askstatus.Domain;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Askstatus.Application.Tests;

public class UploadGoogleTokenResponseFileCommandHandlerTests
{
    private readonly Mock<IFileService> _fileServiceMock;
    private readonly Mock<ILogger<UploadGoogleTokenResponseFileCommandHandler>> _loggerMock;
    private readonly Mock<IOptions<MailSettings>> _optionsMock;
    private readonly UploadGoogleTokenResponseFileCommandHandler _handler;

    public UploadGoogleTokenResponseFileCommandHandlerTests()
    {
        _fileServiceMock = new Mock<IFileService>();
        _loggerMock = new Mock<ILogger<UploadGoogleTokenResponseFileCommandHandler>>();
        _optionsMock = new Mock<IOptions<MailSettings>>();
        _optionsMock.Setup(o => o.Value).Returns(new MailSettings { CredentialCacheFolder = "test_folder" });

        _handler = new UploadGoogleTokenResponseFileCommandHandler(_fileServiceMock.Object, _loggerMock.Object, _optionsMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnOkResult_WhenFileIsSavedSuccessfully()
    {
        // Arrange
        var command = new UploadGoogleTokenResponseFileCommand("test.txt", new MemoryStream());
        _fileServiceMock.Setup(fs => fs.SaveFileAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _fileServiceMock.Verify(fs => fs.SaveFileAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailResult_WhenFileIsNotSaved()
    {
        // Arrange
        var command = new UploadGoogleTokenResponseFileCommand("test.txt", new MemoryStream());
        _fileServiceMock.Setup(fs => fs.SaveFileAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Should().BeOfType<BadRequestError>();
        result.Errors.First().Message.Should().Be("Failed to save file");
        _fileServiceMock.Verify(fs => fs.SaveFileAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Once);
        _loggerMock.Verify(l =>
        l.Log(LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Failed to save file with file name ")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
    }
}
