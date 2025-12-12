using Askstatus.Application.Errors;
using Askstatus.Application.Interfaces;
using Askstatus.Application.Sensors;
using Askstatus.Common.Sensor;
using Askstatus.Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Askstatus.Application.Tests;

public class CreateSensorCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IRepository<Sensor>> _sensorRepoMock = new();
    private readonly Mock<ILogger<CreateSensorCommandHandler>> _loggerMock = new();
    private readonly CreateSensorCommandHandler _handler;

    public CreateSensorCommandHandlerTests()
    {
        _unitOfWorkMock.SetupGet(u => u.SensorRepository).Returns(_sensorRepoMock.Object);
        _handler = new CreateSensorCommandHandler(_unitOfWorkMock.Object, _loggerMock.Object);
    }

    private static CreateSensorCommand CreateValidCommand() => new()
    {
        Name = "TestSensor",
        SensorType = SensorType.Temperature,
        SensorName = "TS-01",
        SensorModel = "ModelX",
        ValueName = "Value1"
    };

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenSensorCreatedSuccessfully()
    {
        var sensor = new Sensor { Id = 1, Name = "TestSensor", SensorType = SensorType.Temperature, FormatString = "°C", SensorName = "TS-01", SensorModel = "ModelX", ValueName = "Value1" };
        _sensorRepoMock.Setup(r => r.AddAsync(It.IsAny<Sensor>())).ReturnsAsync(sensor);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _handler.Handle(CreateValidCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(new SensorDto(sensor.Id, sensor.Name, sensor.SensorType, sensor.FormatString, sensor.SensorName, sensor.SensorModel, sensor.ValueName));
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenAddAsyncReturnsNull()
    {
        Sensor sensor = null!;
        _sensorRepoMock.Setup(r => r.AddAsync(It.IsAny<Sensor>())).ReturnsAsync(sensor);

        var result = await _handler.Handle(CreateValidCommand(), CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is BadRequestError && e.Message == "Error creating Sensor");
        _loggerMock.Verify(l =>
        l.Log(LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Error creating Sensor")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenSaveChangesAsyncReturnsMinusOne()
    {
        var sensor = new Sensor { Id = 1, Name = "TestSensor", SensorType = SensorType.Temperature, SensorName = "TS-01", ValueName = "Value1" };
        _sensorRepoMock.Setup(r => r.AddAsync(It.IsAny<Sensor>())).ReturnsAsync(sensor);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(-1);

        var result = await _handler.Handle(CreateValidCommand(), CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is BadRequestError && e.Message == "Error saving changes");
        _loggerMock.Verify(l =>
        l.Log(LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Error saving changes")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
    }
}
