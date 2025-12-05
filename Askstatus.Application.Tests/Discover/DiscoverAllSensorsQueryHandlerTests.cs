using Askstatus.Application.Errors;
using Askstatus.Application.Interfaces;
using Askstatus.Application.Sensors;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Askstatus.Application.Tests;

public class DiscoverAllSensorsQueryHandlerTests
{
    private readonly Mock<ILogger<DiscoverAllSensorsQueryHandler>> _loggerMock;
    private readonly Mock<IMqttClientService> _mqttClientServiceMock;
    private readonly DiscoverAllSensorsQueryHandler _handler;

    public DiscoverAllSensorsQueryHandlerTests()
    {
        _loggerMock = new Mock<ILogger<DiscoverAllSensorsQueryHandler>>();
        _mqttClientServiceMock = new Mock<IMqttClientService>();
        _handler = new DiscoverAllSensorsQueryHandler(_mqttClientServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSensors_WhenSensorsAreFound()
    {
        // Arrange
        DateTime now = DateTime.UtcNow;
        var sensors = new List<DeviceSensor>
        {
            new DeviceSensor("1", new List<DeviceSensorValue>() { new DeviceSensorValue("temperature", "22.25", now ) }),
            new DeviceSensor("2", new List<DeviceSensorValue>() { new DeviceSensorValue("humidity", "55", now ) }),
            new DeviceSensor("3", new List<DeviceSensorValue>() { new DeviceSensorValue("battery", "100", now ) }),
            new DeviceSensor("4", new List<DeviceSensorValue>() { new DeviceSensorValue("ext_power", "true", now ) }),
        };
        _mqttClientServiceMock.Setup(x => x.GetSensorsAsync()).ReturnsAsync(sensors);

        // Act
        var result = await _handler.Handle(new DiscoverAllSensorsQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(4);
        result.Value.First().Id.Should().Be("1");
        result.Value.First().Values.Should().HaveCount(1);
        result.Value.First().Values.First().Name.Should().Be("temperature");
        result.Value.First().Values.First().Value.Should().Be("22.25");
        result.Value.First().Values.First().LastUpdate.Should().Be(now);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenExceptionIsThrown()
    {
        // Arrange
        _mqttClientServiceMock.Setup(x => x.GetSensorsAsync()).ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _handler.Handle(new DiscoverAllSensorsQuery(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.First().Should().BeOfType<ServerError>();
        result.Errors.First().Message.Should().Be("Failed to get sensors");
        _loggerMock.Verify(l =>
        l.Log(LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Failed to get sensors")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
    }
}
