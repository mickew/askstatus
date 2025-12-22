using Askstatus.Application.Errors;
using Askstatus.Application.Interfaces;
using Askstatus.Application.System;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Askstatus.Application.Tests;

public class UploadProductionAppSettingsFileCommandHandlerTests
{
    private readonly Mock<IFileService> _fileServiceMock;
    private readonly Mock<ILogger<UploadProductionAppSettingsFileCommandHandler>> _loggerMock;
    private readonly UploadProductionAppSettingsFileCommandHandler _handler;

    public UploadProductionAppSettingsFileCommandHandlerTests()
    {
        _fileServiceMock = new Mock<IFileService>();
        _loggerMock = new Mock<ILogger<UploadProductionAppSettingsFileCommandHandler>>();
        _handler = new UploadProductionAppSettingsFileCommandHandler(_fileServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnOkResult_WhenFileIsSavedSuccessfully()
    {
        // Arrange
        var command = new UploadProductionAppSettingsFileCommand("appsettings.Production.json", new MemoryStream());
        _fileServiceMock.Setup(x => x.SaveFileAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailResult_WhenFileIsNotSaved()
    {
        // Arrange
        var command = new UploadProductionAppSettingsFileCommand("appsettings.Production.json", new MemoryStream());
        _fileServiceMock.Setup(x => x.SaveFileAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Should().BeOfType<BadRequestError>();
        result.Errors.First().Message.Should().Be("Failed to save file");
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Failed to save file with file name appsettings.Production.json")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
