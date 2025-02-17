using Askstatus.Application.Interfaces;
using Askstatus.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Askstatus.Tests.Services
{
    public class FileServiceTests
    {
        private readonly Mock<ILogger<FileService>> _loggerMock;
        private readonly IFileService _fileService;

        public FileServiceTests()
        {
            _loggerMock = new Mock<ILogger<FileService>>();
            _fileService = new FileService(_loggerMock.Object);
        }

        [Fact]
        public async Task SaveFileAsync_ShouldReturnTrue_WhenFileIsSavedSuccessfully()
        {
            // Arrange
            var fileName = "testfile.txt";
            var data = new MemoryStream();
            var cancellationToken = CancellationToken.None;

            // Act
            var result = await _fileService.SaveFileAsync(fileName, data, cancellationToken);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task SaveFileAsync_ShouldReturnFalse_WhenExceptionIsThrown()
        {
            // Arrange
            //var fileName = "testfile.txt";
            var data = new MemoryStream();
            var cancellationToken = CancellationToken.None;

            // Simulate an exception by using an invalid file name
            var invalidFileName = string.Empty;

            // Act
            var result = await _fileService.SaveFileAsync(invalidFileName, data, cancellationToken);

            // Assert
            result.Should().BeFalse();
            _loggerMock.Verify(l =>
            l.Log(LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Failed to save file with file name ")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ), Times.Once);
        }
    }
}
