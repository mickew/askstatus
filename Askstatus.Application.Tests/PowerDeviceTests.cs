using Askstatus.Application.Interfaces;
using Askstatus.Application.PowerDevice;
using Askstatus.Common.PowerDevice;
using FluentAssertions;
using Microsoft.Extensions.Logging;
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

    [Fact]
    public async Task GetPowerDeviceById_Should_Return_Success()
    {
        // Arrange
        var powerDevice = new Askstatus.Domain.Entities.PowerDevice
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
        var logger = new Mock<ILogger<GetPowerDeviceByIdQueryHandler>>();
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(x => x.PowerDeviceRepository.GetByIdAsync(1)).ReturnsAsync(powerDevice);
        var handler = new GetPowerDeviceByIdQueryHandler(unitOfWork.Object, logger.Object);
        var query = new GetPowerDeviceByIdQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(1);
        result.Value.Name.Should().Be("PowerDevice1");
        result.Value.DeviceType.Should().Be(PowerDeviceTypes.Generic);
        result.Value.HostName.Should().Be("HostName1");
        result.Value.DeviceName.Should().Be("DeviceName1");
        result.Value.DeviceId.Should().Be("DeviceId1");
        result.Value.DeviceMac.Should().Be("DeviceMac1");
        result.Value.DeviceModel.Should().Be("DeviceModel1");
        result.Value.DeviceGen.Should().Be(1);
        unitOfWork.Verify(x => x.PowerDeviceRepository.GetByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetPowerDeviceById_Should_Return_Failiure()
    {
        // Arrange
        Askstatus.Domain.Entities.PowerDevice powerDevice = null!;
        var logger = new Mock<ILogger<GetPowerDeviceByIdQueryHandler>>();
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(x => x.PowerDeviceRepository.GetByIdAsync(1)).ReturnsAsync(powerDevice);
        var handler = new GetPowerDeviceByIdQueryHandler(unitOfWork.Object, logger.Object);
        var query = new GetPowerDeviceByIdQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);
        // Assert

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Message.Should().Be("PowerDevice not found");
        unitOfWork.Verify(x => x.PowerDeviceRepository.GetByIdAsync(1), Times.Once);
        logger.Verify(l =>
        l.Log(LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("PowerDevice with id 1 not found")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
    }

    [Fact]
    public async Task UpdatePowerDevice_Should_Return_Success()
    {
        // Arrange
        var powerDevice = new Askstatus.Domain.Entities.PowerDevice
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

        var logger = new Mock<ILogger<UpdatePowerDeviceCommandHandler>>();
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(x => x.PowerDeviceRepository.GetByIdAsync(1)).ReturnsAsync(powerDevice);
        unitOfWork.Setup(x => x.PowerDeviceRepository.UpdateAsync(It.IsAny<Askstatus.Domain.Entities.PowerDevice>())).ReturnsAsync(true);
        unitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
        var handler = new UpdatePowerDeviceCommandHandler(unitOfWork.Object, logger.Object);
        var command = new UpdatePowerDeviceCommand
        {
            Id = 1,
            Name = "PowerDevice1New",
            DeviceType = PowerDeviceTypes.ShellyGen2,
            HostName = "HostName1New",
            DeviceName = "DeviceName1Nem",
            DeviceId = "DeviceId1Nem",
            DeviceMac = "DeviceMac1New",
            DeviceModel = "DeviceModel1New",
            DeviceGen = 2,
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        unitOfWork.Verify(x => x.PowerDeviceRepository.GetByIdAsync(1), Times.Once);
        unitOfWork.Verify(x => x.PowerDeviceRepository.UpdateAsync(powerDevice), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdatePowerDevice_Should_Return_NotFound()
    {
        // Arrange
        Askstatus.Domain.Entities.PowerDevice powerDevice = null!;
        var logger = new Mock<ILogger<UpdatePowerDeviceCommandHandler>>();
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(x => x.PowerDeviceRepository.GetByIdAsync(1)).ReturnsAsync(powerDevice);
        var handler = new UpdatePowerDeviceCommandHandler(unitOfWork.Object, logger.Object);
        var command = new UpdatePowerDeviceCommand
        {
            Id = 1,
            Name = "PowerDevice1New",
            DeviceType = PowerDeviceTypes.ShellyGen2,
            HostName = "HostName1New",
            DeviceName = "DeviceName1Nem",
            DeviceId = "DeviceId1Nem",
            DeviceMac = "DeviceMac1New",
            DeviceModel = "DeviceModel1New",
            DeviceGen = 2,
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        // Assert

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Message.Should().Be("PowerDevice not found");
        unitOfWork.Verify(x => x.PowerDeviceRepository.GetByIdAsync(1), Times.Once);
        logger.Verify(l =>
        l.Log(LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("PowerDevice with id 1 not found")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
    }
    [Fact]
    public async Task UpdatePowerDevice_Should_Return_BadRequest()
    {
        // Arrange
        var powerDevice = new Askstatus.Domain.Entities.PowerDevice
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
        var logger = new Mock<ILogger<UpdatePowerDeviceCommandHandler>>();
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(x => x.PowerDeviceRepository.GetByIdAsync(1)).ReturnsAsync(powerDevice);
        unitOfWork.Setup(x => x.PowerDeviceRepository.UpdateAsync(It.IsAny<Askstatus.Domain.Entities.PowerDevice>())).ReturnsAsync(false);
        var handler = new UpdatePowerDeviceCommandHandler(unitOfWork.Object, logger.Object);
        var command = new UpdatePowerDeviceCommand
        {
            Id = 1,
            Name = "PowerDevice1New",
            DeviceType = PowerDeviceTypes.ShellyGen2,
            HostName = "HostName1New",
            DeviceName = "DeviceName1Nem",
            DeviceId = "DeviceId1Nem",
            DeviceMac = "DeviceMac1New",
            DeviceModel = "DeviceModel1New",
            DeviceGen = 2,
        };
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Message.Should().Be("Error updating PowerDevice");
        unitOfWork.Verify(x => x.PowerDeviceRepository.GetByIdAsync(1), Times.Once);
        unitOfWork.Verify(x => x.PowerDeviceRepository.UpdateAsync(powerDevice), Times.Once);
        logger.Verify(l =>
        l.Log(LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Error updating PowerDevice with id 1")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
    }

    [Fact]
    public async Task CreatePowerDevice_Should_Return_Success()
    {
        // Arrange
        var powerDevice = new Askstatus.Domain.Entities.PowerDevice
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
        var logger = new Mock<ILogger<CreatePowerDeviceCommandHandler>>();
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(x => x.PowerDeviceRepository.AddAsync(It.IsAny<Askstatus.Domain.Entities.PowerDevice>())).ReturnsAsync(powerDevice);
        unitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
        var handler = new CreatePowerDeviceCommandHandler(unitOfWork.Object, logger.Object);
        var command = new CreatePowerDeviceCommand
        {
            Name = "PowerDevice1",
            DeviceType = PowerDeviceTypes.Generic,
            HostName = "HostName1",
            DeviceName = "DeviceName1",
            DeviceId = "DeviceId1",
            DeviceMac = "DeviceMac1",
            DeviceModel = "DeviceModel1",
            DeviceGen = 1
        };
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(1);
        result.Value.Name.Should().Be("PowerDevice1");
        result.Value.DeviceType.Should().Be(PowerDeviceTypes.Generic);
        result.Value.HostName.Should().Be("HostName1");
        result.Value.DeviceName.Should().Be("DeviceName1");
        result.Value.DeviceId.Should().Be("DeviceId1");
        result.Value.DeviceMac.Should().Be("DeviceMac1");
        result.Value.DeviceModel.Should().Be("DeviceModel1");
        result.Value.DeviceGen.Should().Be(1);
        unitOfWork.Verify(x => x.PowerDeviceRepository.AddAsync(It.IsAny<Askstatus.Domain.Entities.PowerDevice>()), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreatePowerDevice_Should_Return_BadRequest()
    {
        // Arrange
        Askstatus.Domain.Entities.PowerDevice powerDevice = null!;
        var logger = new Mock<ILogger<CreatePowerDeviceCommandHandler>>();
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(x => x.PowerDeviceRepository.AddAsync(It.IsAny<Askstatus.Domain.Entities.PowerDevice>())).ReturnsAsync(powerDevice);
        var handler = new CreatePowerDeviceCommandHandler(unitOfWork.Object, logger.Object);
        var command = new CreatePowerDeviceCommand
        {
            Name = "PowerDevice1",
            DeviceType = PowerDeviceTypes.Generic,
            HostName = "HostName1",
            DeviceName = "DeviceName1",
            DeviceId = "DeviceId1",
            DeviceMac = "DeviceMac1",
            DeviceModel = "DeviceModel1",
            DeviceGen = 1
        };
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Message.Should().Be("Error creating PowerDevice");
        unitOfWork.Verify(x => x.PowerDeviceRepository.AddAsync(It.IsAny<Askstatus.Domain.Entities.PowerDevice>()), Times.Once);
        logger.Verify(l =>
        l.Log(LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Error creating PowerDevice")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
    }

    [Fact]
    public async Task DeletePowerDevice_Should_Return_Success()
    {
        // Arrange
        var powerDevice = new Askstatus.Domain.Entities.PowerDevice
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
        var logger = new Mock<ILogger<DeletePowerDeviceCommandHandler>>();
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(x => x.PowerDeviceRepository.GetByIdAsync(1)).ReturnsAsync(powerDevice);
        unitOfWork.Setup(x => x.PowerDeviceRepository.DeleteAsync(powerDevice)).ReturnsAsync(true);
        unitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
        var handler = new DeletePowerDeviceCommandHandler(unitOfWork.Object, logger.Object);
        var command = new DeletePowerDeviceCommand(1);
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        // Assert
        result.IsSuccess.Should().BeTrue();
        unitOfWork.Verify(x => x.PowerDeviceRepository.GetByIdAsync(1), Times.Once);
        unitOfWork.Verify(x => x.PowerDeviceRepository.DeleteAsync(powerDevice), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeletePowerDevice_Should_Return_NotFound()
    {
        // Arrange
        Askstatus.Domain.Entities.PowerDevice powerDevice = null!;
        var logger = new Mock<ILogger<DeletePowerDeviceCommandHandler>>();
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(x => x.PowerDeviceRepository.GetByIdAsync(1)).ReturnsAsync(powerDevice);
        var handler = new DeletePowerDeviceCommandHandler(unitOfWork.Object, logger.Object);
        var command = new DeletePowerDeviceCommand(1);
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Message.Should().Be("PowerDevice not found");
        unitOfWork.Verify(x => x.PowerDeviceRepository.GetByIdAsync(1), Times.Once);
        logger.Verify(l =>
        l.Log(LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("PowerDevice with id 1 not found")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
    }

    [Fact]
    public async Task DeletePowerDevice_Should_Return_BadRequest()
    {
        // Arrange
        var powerDevice = new Askstatus.Domain.Entities.PowerDevice
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
        var logger = new Mock<ILogger<DeletePowerDeviceCommandHandler>>();
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(x => x.PowerDeviceRepository.GetByIdAsync(1)).ReturnsAsync(powerDevice);
        unitOfWork.Setup(x => x.PowerDeviceRepository.DeleteAsync(powerDevice)).ReturnsAsync(false);
        var handler = new DeletePowerDeviceCommandHandler(unitOfWork.Object, logger.Object);
        var command = new DeletePowerDeviceCommand(1);
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Message.Should().Be("Error deleting PowerDevice");
        unitOfWork.Verify(x => x.PowerDeviceRepository.GetByIdAsync(1), Times.Once);
        unitOfWork.Verify(x => x.PowerDeviceRepository.DeleteAsync(powerDevice), Times.Once);
        logger.Verify(l =>
        l.Log(LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Error deleting PowerDevice with id 1")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
    }
}
