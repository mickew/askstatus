using Askstatus.Application.Errors;
using Askstatus.Application.Interfaces;
using Askstatus.Application.Sensors;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Askstatus.Application.Tests;

public class DiscoverSensorsQueryHandlerTests
{
    private readonly Mock<ILogger<DiscoverSensorsQueryHandler>> _loggerMock = new();
    private readonly Mock<IMqttClientService> _mqttClientServiceMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IRepository<Askstatus.Domain.Entities.Sensor>> _sensorRepoMock = new();
    private readonly DiscoverSensorsQueryHandler _handler;

    public DiscoverSensorsQueryHandlerTests()
    {
        _unitOfWorkMock.SetupGet(u => u.SensorRepository).Returns(_sensorRepoMock.Object);
        _handler = new DiscoverSensorsQueryHandler(_loggerMock.Object, _mqttClientServiceMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsSensors_WhenSensorsExist()
    {
        // Arrange
        var assignedSensors = new List<Askstatus.Domain.Entities.Sensor>
        {
            new Askstatus.Domain.Entities.Sensor { SensorName = "sensor1", ValueName = "temp" },
            new Askstatus.Domain.Entities.Sensor { SensorName = "sensor2", ValueName = "hum" }
        };
        var sensors = new List<DeviceSensor>
        {
            new DeviceSensor("sensor1", new List<DeviceSensorValue> {
                new DeviceSensorValue("temp", "10", DateTime.UtcNow),
                new DeviceSensorValue("hum", "20", DateTime.UtcNow)
            }),
            new DeviceSensor("sensor3", new List<DeviceSensorValue> {
                new DeviceSensorValue("temp", "30", DateTime.UtcNow)
            })
        };
        _sensorRepoMock.Setup(r => r.ListAllAsync()).ReturnsAsync(assignedSensors);
        _mqttClientServiceMock.Setup(m => m.GetSensorsAsync()).ReturnsAsync(sensors);

        // Act
        var result = await _handler.Handle(new DiscoverSensorsQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var sensorInfos = result.Value.ToList();
        sensorInfos.Should().HaveCount(2);
        sensorInfos[0].Id.Should().Be("sensor1");
        sensorInfos[0].Values.Should().ContainSingle().Which.Name.Should().Be("hum");
        sensorInfos[1].Id.Should().Be("sensor3");
        sensorInfos[1].Values.Should().ContainSingle().Which.Name.Should().Be("temp");
    }

    [Fact]
    public async Task Handle_ReturnsFail_WhenExceptionThrown()
    {
        // Arrange
        _sensorRepoMock.Setup(r => r.ListAllAsync()).ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _handler.Handle(new DiscoverSensorsQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.First().Should().BeOfType<ServerError>();
    }

    [Fact]
    public async Task Handle_ReturnsEmpty_WhenNoSensors()
    {
        // Arrange
        _sensorRepoMock.Setup(r => r.ListAllAsync()).ReturnsAsync(new List<Askstatus.Domain.Entities.Sensor>());
        _mqttClientServiceMock.Setup(m => m.GetSensorsAsync()).ReturnsAsync(new List<DeviceSensor>());

        // Act
        var result = await _handler.Handle(new DiscoverSensorsQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ReturnsAllValues_WhenNoAssignedSensors()
    {
        // Arrange
        _sensorRepoMock.Setup(r => r.ListAllAsync()).ReturnsAsync(new List<Askstatus.Domain.Entities.Sensor>());
        var sensors = new List<DeviceSensor>
        {
            new DeviceSensor("sensor1", new List<DeviceSensorValue> {
                new DeviceSensorValue("temp", "10", DateTime.UtcNow)
            })
        };
        _mqttClientServiceMock.Setup(m => m.GetSensorsAsync()).ReturnsAsync(sensors);

        // Act
        var result = await _handler.Handle(new DiscoverSensorsQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var sensorInfos = result.Value.ToList();
        sensorInfos.Should().ContainSingle();
        sensorInfos[0].Id.Should().Be("sensor1");
        sensorInfos[0].Values.Should().ContainSingle().Which.Name.Should().Be("temp");
    }
}
