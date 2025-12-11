using Askstatus.Application.Errors;
using Askstatus.Application.Interfaces;
using Askstatus.Application.Sensors;
using Askstatus.Common.Sensor;
using Askstatus.Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Askstatus.Application.Tests;

public class UpdateSensorCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IRepository<Sensor>> _sensorRepositoryMock;
    private readonly Mock<ILogger<UpdateSensorCommandHandler>> _loggerMock;
    private readonly UpdateSensorCommandHandler _handler;

    public UpdateSensorCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _sensorRepositoryMock = new Mock<IRepository<Sensor>>();
        _loggerMock = new Mock<ILogger<UpdateSensorCommandHandler>>();
        _unitOfWorkMock.SetupGet(u => u.SensorRepository).Returns(_sensorRepositoryMock.Object);
        _handler = new UpdateSensorCommandHandler(_unitOfWorkMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenSensorDoesNotExist()
    {
        // Arrange
        var command = new UpdateSensorCommand { Id = 1 };
        _sensorRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Sensor?)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is NotFoundError);
        _loggerMock.Verify(l => l.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v != null && v.ToString() != null && v.ToString()!.Contains($"Sensor with id {command.Id} not found")),
            null,
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnBadRequest_WhenUpdateFails()
    {
        // Arrange
        var command = new UpdateSensorCommand { Id = 2, Name = "n", SensorType = SensorType.Temperature, FormatString = "fs", SensorName = "sn", ValueName = "vn" };
        var sensor = new Sensor { Id = 2, Name = "old", SensorType = SensorType.Humidity, SensorName = "oldsn", ValueName = "oldvn" };
        _sensorRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(sensor);
        _sensorRepositoryMock.Setup(r => r.UpdateAsync(sensor)).ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is BadRequestError);
        _loggerMock.Verify(l => l.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v != null && v.ToString() != null && v.ToString()!.Contains("Error updating Sensor")),
            null,
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnBadRequest_WhenSaveChangesFails()
    {
        // Arrange
        var command = new UpdateSensorCommand { Id = 3, Name = "n", SensorType = SensorType.Temperature, FormatString = "fs", SensorName = "sn", SensorModel = "sm", ValueName = "vn" };
        var sensor = new Sensor { Id = 3, Name = "old", SensorType = SensorType.Humidity, FormatString = "oldfs", SensorName = "oldsn", SensorModel = "oldsm", ValueName = "oldvn" };
        _sensorRepositoryMock.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(sensor);
        _sensorRepositoryMock.Setup(r => r.UpdateAsync(sensor)).ReturnsAsync(true);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(-1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is BadRequestError);
        _loggerMock.Verify(l => l.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v != null && v.ToString() != null && v.ToString()!.Contains("Error saving changes")),
            null,
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenUpdateSucceeds()
    {
        // Arrange
        var command = new UpdateSensorCommand { Id = 4, Name = "n", SensorType = SensorType.Temperature, FormatString = "fs", SensorName = "sn", SensorModel = "sm", ValueName = "vn" };
        var sensor = new Sensor { Id = 4, Name = "old", SensorType = SensorType.Humidity, FormatString = "oldfs", SensorName = "oldsn", SensorModel = "oldsm", ValueName = "oldvn" };
        _sensorRepositoryMock.Setup(r => r.GetByIdAsync(4)).ReturnsAsync(sensor);
        _sensorRepositoryMock.Setup(r => r.UpdateAsync(sensor)).ReturnsAsync(true);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
}
