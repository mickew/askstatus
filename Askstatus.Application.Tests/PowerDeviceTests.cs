using Askstatus.Application.Interfaces;
using Askstatus.Application.PowerDevice;
using Askstatus.Common.PowerDevice;
using FluentAssertions;
using Moq;

namespace Askstatus.Application.Tests;
public class PowerDeviceTests
{
    [Fact]
    public async Task GetPowerDevice_Should_Return_Success()
    {
        // Arrange
        var powerDevice1 = new Askstatus.Domain.Entities.PowerDevice
        {
            Id = 1,
            Name = "PowerDevice1",
            DeviceType = PowerDeviceTypes.Generic,
            HostName = "HostName1",
            DeviceName = "DeviceName1",
            DeviceId = "DeviceId1",
            DeviceMac = "DeviceMac1",
            DeviceModel = "DeviceModel1",
            DeviceGen = 1
        };
        var poweDevice2 = new Askstatus.Domain.Entities.PowerDevice
        {
            Id = 2,
            Name = "PowerDevice2",
            DeviceType = PowerDeviceTypes.Generic,
            HostName = "HostName2",
            DeviceName = "DeviceName2",
            DeviceId = "DeviceId2",
            DeviceMac = "DeviceMac2",
            DeviceModel = "DeviceModel2",
            DeviceGen = 2
        };
        var powerDevices = new List<Askstatus.Domain.Entities.PowerDevice> { powerDevice1, poweDevice2 };
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(x => x.PowerDeviceRepository.ListAllAsync()).ReturnsAsync(powerDevices);
        var handler = new GetPowerDevicesQueryHandler(unitOfWork.Object);
        var query = new GetPowerDevicesQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(2);
    }
}
