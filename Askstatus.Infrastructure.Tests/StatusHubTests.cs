using Askstatus.Application.PowerDevice;
using Askstatus.Infrastructure.Events;
using Askstatus.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using SignalR_UnitTestingSupportCommon.IHubContextSupport;

namespace Askstatus.Infrastructure.Tests;

public class StatusHubTests
{
    private readonly Mock<ILogger<StatusHub>> _mockLogger;
    private readonly Mock<HubCallerContext> _mockContext;
    private readonly StatusHub _statusHub;

    public StatusHubTests()
    {
        _mockLogger = new Mock<ILogger<StatusHub>>();
        _mockContext = new Mock<HubCallerContext>();
        _statusHub = new StatusHub(_mockLogger.Object)
        {
            Context = _mockContext.Object
        };
    }

    [Fact]
    public async Task StatusHub_ShouldUpdateAllClient_WhenDeviceStateChangedIntegrationEventIsFired()
    {
        // Arrange
        //var loggerMock = new Mock<ILogger<StatusHub>>();
        var logger = new Mock<ILogger<DeviceStateUpdateEventHandler>>();
        var iHubContextSupport
                = new UnitTestingSupportForIHubContext<StatusHub, IStatusClient>();
        var exampleService
                = new DeviceStateUpdateEventHandler(logger.Object, iHubContextSupport.IHubContextMock.Object);
        var deviceStateChangedIntegrationEvent = new DeviceStateChangedIntegrationEvent(Guid.NewGuid(), 1, true);

        // Act
        await exampleService.Handle(deviceStateChangedIntegrationEvent, new System.Threading.CancellationToken());

        // Assert
        iHubContextSupport.ClientsAllMock.Verify(
            x => x.UpdateDeviceStatus(deviceStateChangedIntegrationEvent.DeviceId, deviceStateChangedIntegrationEvent.State),
            Times.Once);
    }

    [Fact]
    public async Task OnConnectedAsync_ShouldLogInformation()
    {
        // Arrange
        var connectionId = "test-connection-id";
        _mockContext.Setup(c => c.ConnectionId).Returns(connectionId);

        // Act
        await _statusHub.OnConnectedAsync();

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Client connected: {connectionId}")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task OnDisconnectedAsync_ShouldLogInformation()
    {
        // Arrange
        var connectionId = "test-connection-id";
        _mockContext.Setup(c => c.ConnectionId).Returns(connectionId);
        Exception? exception = null;

        // Act
        await _statusHub.OnDisconnectedAsync(exception);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Client disconnected: {connectionId}")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
