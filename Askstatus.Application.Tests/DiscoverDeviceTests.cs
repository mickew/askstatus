using Askstatus.Application.DiscoverDevice;
using Askstatus.Application.Interfaces;
using Askstatus.Common.Models;
using Askstatus.Common.PowerDevice;
using FluentAssertions;
using FluentResults;
using Microsoft.Extensions.Logging;
using Moq;

namespace Askstatus.Application.Tests;
public class DiscoverDeviceTests
{
    [Fact]
    public async Task Discover_Should_Return_DicoverInfAsync()
    {
        // Arrange
        var logger = new Mock<ILogger<ShellyDiscoverDeviceQueryHandler>>();
        var discoverInfo = new DicoverInfo("DeviceHostName", PowerDeviceTypes.ShellyGen1, "DeviceName", "DeviceId", "DeviceMac", "DeviceModel", 1);
        var discoverDeviceService = new Mock<IDiscoverDeviceService>();
        discoverDeviceService.Setup(x => x.Discover(It.IsAny<string>())).ReturnsAsync(Result.Ok(discoverInfo));
        var shellyDiscoverDeviceQuery = new ShellyDiscoverDeviceQuery(string.Empty);
        var handler = new ShellyDiscoverDeviceQueryHandler(logger.Object, discoverDeviceService.Object);

        // Act
        var result = await handler.Handle(shellyDiscoverDeviceQuery, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(discoverInfo);
    }
}
