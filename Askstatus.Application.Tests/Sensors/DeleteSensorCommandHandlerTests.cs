using Askstatus.Application.Errors;
using Askstatus.Application.Interfaces;
using Askstatus.Application.Sensors;
using Askstatus.Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Askstatus.Application.Tests;

public class DeleteSensorCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IRepository<Sensor>> _sensorRepoMock = new();
    private readonly Mock<ILogger<DeleteSensorCommandHandler>> _loggerMock = new();
    private readonly DeleteSensorCommandHandler _handler;

    public DeleteSensorCommandHandlerTests()
    {
        _unitOfWorkMock.SetupGet(u => u.SensorRepository).Returns(_sensorRepoMock.Object);
        _handler = new DeleteSensorCommandHandler(_unitOfWorkMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenSensorDoesNotExist()
    {
        Sensor sensor = null!;
        _sensorRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(sensor);
        var command = new DeleteSensorCommand(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is NotFoundError && e.Message == "Sensor not found");
        _loggerMock.Verify(l => l.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Sensor with id 1 not found")),
            null,
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenDeleteFails()
    {
        var sensor = new Sensor { Id = 1 };
        _sensorRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(sensor);
        _sensorRepoMock.Setup(r => r.DeleteAsync(sensor)).ReturnsAsync(false);
        var command = new DeleteSensorCommand(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is BadRequestError && e.Message == "Error deleting Sensor");
        _loggerMock.Verify(l => l.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Error deleting Sensor")),
            null,
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenSaveChangesFails()
    {
        var sensor = new Sensor { Id = 1 };
        _sensorRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(sensor);
        _sensorRepoMock.Setup(r => r.DeleteAsync(sensor)).ReturnsAsync(true);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(-1);
        var command = new DeleteSensorCommand(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is BadRequestError && e.Message == "Error saving changes");
        _loggerMock.Verify(l => l.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Error saving changes")),
            null,
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenDeleteSucceeds()
    {
        var sensor = new Sensor { Id = 1 };
        _sensorRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(sensor);
        _sensorRepoMock.Setup(r => r.DeleteAsync(sensor)).ReturnsAsync(true);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        var command = new DeleteSensorCommand(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }
}
