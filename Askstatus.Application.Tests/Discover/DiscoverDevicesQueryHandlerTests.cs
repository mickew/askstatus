using Askstatus.Application.DiscoverDevice;
using Askstatus.Application.Errors;
using Askstatus.Application.Interfaces;
using Askstatus.Common.PowerDevice;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Askstatus.Application.Tests;

public class DiscoverDevicesQueryHandlerTests
{
    private readonly Mock<ILogger<DiscoverDevicesQueryHandler>> _loggerMock;
    private readonly Mock<IMqttClientService> _mqttClientServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly DiscoverDevicesQueryHandler _handler;

    public DiscoverDevicesQueryHandlerTests()
    {
        _loggerMock = new Mock<ILogger<DiscoverDevicesQueryHandler>>();
        _mqttClientServiceMock = new Mock<IMqttClientService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new DiscoverDevicesQueryHandler(_loggerMock.Object, _mqttClientServiceMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnDevices_WhenDevicesAreNotAssigned()
    {
        // Arrange
        var assignedDevices = new List<Askstatus.Domain.Entities.PowerDevice>
        {
            new Askstatus.Domain.Entities.PowerDevice { DeviceId = "0" }
        };
        var devices = new List<ShellieAnnounce>
        {
            new ShellieAnnounce("Id1", "Name1", "Model1", "00:11:22:33:44:55", "192.168.1.2", 2),
            new ShellieAnnounce("Id2", "Name2", "Model2", "66:77:88:99:AA:BB", "192.168.1.3", 0),
        };

        _unitOfWorkMock.Setup(x => x.PowerDeviceRepository.ListAllAsync()).ReturnsAsync(assignedDevices);
        _mqttClientServiceMock.Setup(x => x.GetShellieDevicesAsync()).ReturnsAsync(devices);

        // Act
        var result = await _handler.Handle(new DiscoverDevicesQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.First().DeviceId.Should().Be("Id1");
        result.Value.First().DeviceName.Should().Be("Name1");
        result.Value.First().DeviceModel.Should().Be("Model1");
        result.Value.First().DeviceMac.Should().Be("00:11:22:33:44:55");
        result.Value.First().DeviceHostName.Should().Be("192.168.1.2");
        result.Value.First().DeviceType.Should().Be(PowerDeviceTypes.ShellyGen2);
        result.Value.First().Channel.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenAllDevicesAreAssigned()
    {
        // Arrange
        var assignedDevices = new List<Askstatus.Domain.Entities.PowerDevice>
        {
            new Askstatus.Domain.Entities.PowerDevice { DeviceId = "Id1" }
        };
        var devices = new List<ShellieAnnounce>
        {
            new ShellieAnnounce("Id1", "Name1", "Model1", "00:11:22:33:44:55", "192.168.1.2", 2),
        };

        _unitOfWorkMock.Setup(x => x.PowerDeviceRepository.ListAllAsync()).ReturnsAsync(assignedDevices);
        _mqttClientServiceMock.Setup(x => x.GetShellieDevicesAsync()).ReturnsAsync(devices);

        // Act
        var result = await _handler.Handle(new DiscoverDevicesQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenExceptionIsThrown()
    {
        // Arrange
        _unitOfWorkMock.Setup(x => x.PowerDeviceRepository.ListAllAsync()).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _handler.Handle(new DiscoverDevicesQuery(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.First().Should().BeOfType<ServerError>();
        result.Errors.First().Message.Should().Be("Failed to get devices");
        _loggerMock.Verify(l =>
        l.Log(LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Failed to get devices")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
    }
}
