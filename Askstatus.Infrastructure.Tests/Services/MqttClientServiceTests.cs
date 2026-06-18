using Askstatus.Domain;
using Askstatus.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Askstatus.Infrastructure.Tests;

public class MqttClientServiceTests
{
    private readonly Mock<ILogger<MqttClientService>> _loggerMock;
    private readonly Mock<IServiceProvider> _serviceProviderMock;

    public MqttClientServiceTests()
    {
        _loggerMock = new Mock<ILogger<MqttClientService>>();
        _serviceProviderMock = new Mock<IServiceProvider>();
    }

    private static IOptions<AskstatusApiSettings> CreateApiOptions(
        string? mqttServer = "localhost",
        int mqttPort = 1883,
        string? mqttClientId = "test-client")
    {
        var settings = new AskstatusApiSettings
        {
            MQTTServer = mqttServer,
            MQTTPort = mqttPort,
            MQTTClientId = mqttClientId,
            BackendUrl = "http://localhost",
            FrontendUrl = "http://localhost"
        };
        return Options.Create(settings);
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange
        var apiOptions = CreateApiOptions();

        // Act
        var service = new MqttClientService(_loggerMock.Object, apiOptions, _serviceProviderMock.Object);

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithCustomPort_ShouldCreateInstance()
    {
        // Arrange
        var apiOptions = CreateApiOptions(mqttPort: 8883);

        // Act
        var service = new MqttClientService(_loggerMock.Object, apiOptions, _serviceProviderMock.Object);

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithCustomClientId_ShouldCreateInstance()
    {
        // Arrange
        var apiOptions = CreateApiOptions(mqttClientId: "custom-client-id");

        // Act
        var service = new MqttClientService(_loggerMock.Object, apiOptions, _serviceProviderMock.Object);

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public async Task GetSensorsAsync_WhenNoSensorsAdded_ShouldReturnEmptyCollection()
    {
        // Arrange
        var apiOptions = CreateApiOptions();
        var service = new MqttClientService(_loggerMock.Object, apiOptions, _serviceProviderMock.Object);

        // Act
        var result = await service.GetSensorsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetShellieDevicesAsync_WhenNoDevicesAdded_ShouldReturnEmptyCollection()
    {
        // Arrange
        var apiOptions = CreateApiOptions();
        var service = new MqttClientService(_loggerMock.Object, apiOptions, _serviceProviderMock.Object);

        // Act
        var result = await service.GetShellieDevicesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task SwitchDeviceAsync_WhenClientNotConnected_ShouldThrow()
    {
        // Arrange
        var apiOptions = CreateApiOptions();
        var service = new MqttClientService(_loggerMock.Object, apiOptions, _serviceProviderMock.Object);

        // Act
        Func<Task> act = async () => await service.SwitchDeviceAsync("device1", 0, true);

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task SwitchDeviceAsync_WithStateFalse_WhenClientNotConnected_ShouldThrow()
    {
        // Arrange
        var apiOptions = CreateApiOptions();
        var service = new MqttClientService(_loggerMock.Object, apiOptions, _serviceProviderMock.Object);

        // Act
        Func<Task> act = async () => await service.SwitchDeviceAsync("device2", 1, false);

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task ToggleDeviceAsync_WhenClientNotConnected_ShouldThrow()
    {
        // Arrange
        var apiOptions = CreateApiOptions();
        var service = new MqttClientService(_loggerMock.Object, apiOptions, _serviceProviderMock.Object);

        // Act
        Func<Task> act = async () => await service.ToggleDeviceAsync("device1", 0);

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task ToggleDeviceAsync_WithDifferentSwitchId_WhenClientNotConnected_ShouldThrow()
    {
        // Arrange
        var apiOptions = CreateApiOptions();
        var service = new MqttClientService(_loggerMock.Object, apiOptions, _serviceProviderMock.Object);

        // Act
        Func<Task> act = async () => await service.ToggleDeviceAsync("device3", 5);

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task GetSensorsAsync_CalledMultipleTimes_ShouldReturnEmptyEachTime()
    {
        // Arrange
        var apiOptions = CreateApiOptions();
        var service = new MqttClientService(_loggerMock.Object, apiOptions, _serviceProviderMock.Object);

        // Act
        var result1 = await service.GetSensorsAsync();
        var result2 = await service.GetSensorsAsync();

        // Assert
        result1.Should().BeEmpty();
        result2.Should().BeEmpty();
    }

    [Fact]
    public async Task GetShellieDevicesAsync_CalledMultipleTimes_ShouldReturnEmptyEachTime()
    {
        // Arrange
        var apiOptions = CreateApiOptions();
        var service = new MqttClientService(_loggerMock.Object, apiOptions, _serviceProviderMock.Object);

        // Act
        var result1 = await service.GetShellieDevicesAsync();
        var result2 = await service.GetShellieDevicesAsync();

        // Assert
        result1.Should().BeEmpty();
        result2.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_ShouldImplementIMqttClientService()
    {
        // Arrange
        var apiOptions = CreateApiOptions();

        // Act
        var service = new MqttClientService(_loggerMock.Object, apiOptions, _serviceProviderMock.Object);

        // Assert
        service.Should().BeAssignableTo<Askstatus.Application.Interfaces.IMqttClientService>();
    }

    [Fact]
    public async Task StartAsync_WhenCalled_ShouldCompleteWithoutThrowing()
    {
        // Arrange
        var apiOptions = CreateApiOptions();
        var service = new MqttClientService(_loggerMock.Object, apiOptions, _serviceProviderMock.Object);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // Act
        Func<Task> act = async () => await service.StartAsync(cts.Token);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task StartAsync_WhenConnectFails_ShouldCatchExceptionAndComplete()
    {
        // Arrange
        var apiOptions = CreateApiOptions();
        var service = new MqttClientService(_loggerMock.Object, apiOptions, _serviceProviderMock.Object);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // Act
        await service.StartAsync(cts.Token);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("MqttClientService starting...")),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("MqttClientService started")),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task StartAsync_WithNonCancelledToken_ShouldStartBackgroundTask()
    {
        // Arrange
        var apiOptions = CreateApiOptions();
        var service = new MqttClientService(_loggerMock.Object, apiOptions, _serviceProviderMock.Object);
        using var cts = new CancellationTokenSource();

        // Act
        await service.StartAsync(cts.Token);

        // Assert - allow background task to run briefly then cancel
        await Task.Delay(100);
        await cts.CancelAsync();

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("MqttClientService started")),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task StopAsync_WhenNotCancelledAndNotConnected_ShouldCompleteNormally()
    {
        // Arrange
        var apiOptions = CreateApiOptions();
        var service = new MqttClientService(_loggerMock.Object, apiOptions, _serviceProviderMock.Object);
        using var cts = new CancellationTokenSource();

        // Act
        await service.StopAsync(cts.Token);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("MqttClientService stopping")),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("MqttClientService stopped")),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task StopAsync_WhenCancelled_ShouldAttemptDisconnectWithOptions()
    {
        // Arrange
        var apiOptions = CreateApiOptions();
        var service = new MqttClientService(_loggerMock.Object, apiOptions, _serviceProviderMock.Object);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // Act
        await service.StopAsync(cts.Token);

        // Assert - verifies that code path through cancelled token branch executed and completed
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("MqttClientService stopping")),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("MqttClientService stopped")),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task RefreshShellieDevicesAsync_WhenClientNotConnected_ShouldThrow()
    {
        // Arrange
        var apiOptions = CreateApiOptions();
        var service = new MqttClientService(_loggerMock.Object, apiOptions, _serviceProviderMock.Object);

        // Act
        Func<Task> act = async () => await service.RefreshShellieDevicesAsync();

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task RefreshStatusAsync_WhenClientNotConnected_ShouldThrow()
    {
        // Arrange
        var apiOptions = CreateApiOptions();
        var service = new MqttClientService(_loggerMock.Object, apiOptions, _serviceProviderMock.Object);

        // Act
        Func<Task> act = async () => await service.RefreshStatusAsync();

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }
}
