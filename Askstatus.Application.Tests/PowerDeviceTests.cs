using System.Linq.Expressions;
using Askstatus.Application.Errors;
using Askstatus.Application.Interfaces;
using Askstatus.Application.PowerDevice;
using Askstatus.Common.PowerDevice;
using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace Askstatus.Application.Tests;
public class PowerDeviceTests
{
    [Fact]
    public async Task GetPowerDevice_Should_Return_Success()
    {
        // Arrange
        var powerDevices = MockTestdata();
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
        var powerDevices = MockTestdata();
        var logger = new Mock<ILogger<GetPowerDeviceByIdQueryHandler>>();
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(x => x.PowerDeviceRepository.GetByIdAsync(1)).ReturnsAsync(powerDevices.First());
        var handler = new GetPowerDeviceByIdQueryHandler(unitOfWork.Object, logger.Object);
        var query = new GetPowerDeviceByIdQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(powerDevices.First().Id);
        result.Value.Name.Should().Be(powerDevices.First().Name);
        result.Value.DeviceType.Should().Be(powerDevices.First().DeviceType);
        result.Value.HostName.Should().Be(powerDevices.First().HostName);
        result.Value.DeviceName.Should().Be(powerDevices.First().DeviceName);
        result.Value.DeviceId.Should().Be(powerDevices.First().DeviceId);
        result.Value.DeviceMac.Should().Be(powerDevices.First().DeviceMac);
        result.Value.DeviceModel.Should().Be(powerDevices.First().DeviceModel);
        result.Value.DeviceGen.Should().Be(powerDevices.First().DeviceGen);
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
        var powerDevices = MockTestdata();

        var logger = new Mock<ILogger<UpdatePowerDeviceCommandHandler>>();
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(x => x.PowerDeviceRepository.GetByIdAsync(1)).ReturnsAsync(powerDevices.First());
        unitOfWork.Setup(x => x.PowerDeviceRepository.UpdateAsync(It.IsAny<Askstatus.Domain.Entities.PowerDevice>())).ReturnsAsync(true);
        unitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
        var handler = new UpdatePowerDeviceCommandHandler(unitOfWork.Object, logger.Object);
        var command = new UpdatePowerDeviceCommand
        {
            Id = powerDevices.First().Id,
            Name = powerDevices.First().Name,
            DeviceType = powerDevices.First().DeviceType,
            HostName = powerDevices.First().HostName,
            DeviceName = powerDevices.First().DeviceName,
            DeviceId = powerDevices.First().DeviceId,
            DeviceMac = powerDevices.First().DeviceMac,
            DeviceModel = powerDevices.First().DeviceModel,
            DeviceGen = 1
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        unitOfWork.Verify(x => x.PowerDeviceRepository.GetByIdAsync(1), Times.Once);
        unitOfWork.Verify(x => x.PowerDeviceRepository.UpdateAsync(powerDevices.First()), Times.Once);
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
        var powerDevices = MockTestdata();
        var logger = new Mock<ILogger<UpdatePowerDeviceCommandHandler>>();
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(x => x.PowerDeviceRepository.GetByIdAsync(1)).ReturnsAsync(powerDevices.First());
        unitOfWork.Setup(x => x.PowerDeviceRepository.UpdateAsync(It.IsAny<Askstatus.Domain.Entities.PowerDevice>())).ReturnsAsync(false);
        var handler = new UpdatePowerDeviceCommandHandler(unitOfWork.Object, logger.Object);
        var command = new UpdatePowerDeviceCommand
        {
            Id = powerDevices.First().Id,
            Name = powerDevices.First().Name,
            DeviceType = powerDevices.First().DeviceType,
            HostName = powerDevices.First().HostName,
            DeviceName = powerDevices.First().DeviceName,
            DeviceId = powerDevices.First().DeviceId,
            DeviceMac = powerDevices.First().DeviceMac,
            DeviceModel = powerDevices.First().DeviceModel,
            DeviceGen = 1
        };
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Message.Should().Be("Error updating PowerDevice");
        unitOfWork.Verify(x => x.PowerDeviceRepository.GetByIdAsync(1), Times.Once);
        unitOfWork.Verify(x => x.PowerDeviceRepository.UpdateAsync(powerDevices.First()), Times.Once);
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
        var powerDevices = MockTestdata();
        var logger = new Mock<ILogger<CreatePowerDeviceCommandHandler>>();
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(x => x.PowerDeviceRepository.AddAsync(It.IsAny<Askstatus.Domain.Entities.PowerDevice>())).ReturnsAsync(powerDevices.First());
        unitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
        var handler = new CreatePowerDeviceCommandHandler(unitOfWork.Object, logger.Object);
        var command = new CreatePowerDeviceCommand
        {
            Name = powerDevices.First().Name,
            DeviceType = powerDevices.First().DeviceType,
            HostName = powerDevices.First().HostName,
            DeviceName = powerDevices.First().DeviceName,
            DeviceId = powerDevices.First().DeviceId,
            DeviceMac = powerDevices.First().DeviceMac,
            DeviceModel = powerDevices.First().DeviceModel,
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
        var powerDevices = MockTestdata();
        var logger = new Mock<ILogger<DeletePowerDeviceCommandHandler>>();
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(x => x.PowerDeviceRepository.GetByIdAsync(1)).ReturnsAsync(powerDevices.First());
        unitOfWork.Setup(x => x.PowerDeviceRepository.DeleteAsync(powerDevices.First())).ReturnsAsync(true);
        unitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
        var handler = new DeletePowerDeviceCommandHandler(unitOfWork.Object, logger.Object);
        var command = new DeletePowerDeviceCommand(1);
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        // Assert
        result.IsSuccess.Should().BeTrue();
        unitOfWork.Verify(x => x.PowerDeviceRepository.GetByIdAsync(1), Times.Once);
        unitOfWork.Verify(x => x.PowerDeviceRepository.DeleteAsync(powerDevices.First()), Times.Once);
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
        var powerDevices = MockTestdata();
        var logger = new Mock<ILogger<DeletePowerDeviceCommandHandler>>();
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(x => x.PowerDeviceRepository.GetByIdAsync(1)).ReturnsAsync(powerDevices.First());
        unitOfWork.Setup(x => x.PowerDeviceRepository.DeleteAsync(powerDevices.First())).ReturnsAsync(false);
        var handler = new DeletePowerDeviceCommandHandler(unitOfWork.Object, logger.Object);
        var command = new DeletePowerDeviceCommand(1);
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Message.Should().Be("Error deleting PowerDevice");
        unitOfWork.Verify(x => x.PowerDeviceRepository.GetByIdAsync(1), Times.Once);
        unitOfWork.Verify(x => x.PowerDeviceRepository.DeleteAsync(powerDevices.First()), Times.Once);
        logger.Verify(l =>
        l.Log(LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Error deleting PowerDevice with id 1")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
    }

    [Fact]
    public async Task TogglePowerDevice_Should_Return_Success()
    {
        // Arrange
        var powerDevices = MockTestdata();
        var logger = new Mock<ILogger<TogglePowerDeviceCommandHandler>>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var deviceService = new Mock<IDeviceService>();

        unitOfWork.Setup(x => x.PowerDeviceRepository.GetByIdAsync(1)).ReturnsAsync(powerDevices.First());
        deviceService.Setup(x => x.Toggle(It.IsAny<string>(), 0)).ReturnsAsync(Result.Ok());
        var handler = new TogglePowerDeviceCommandHandler(unitOfWork.Object, logger.Object, deviceService.Object);
        var command = new TogglePowerDeviceCommand(1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        unitOfWork.Verify(x => x.PowerDeviceRepository.GetByIdAsync(1), Times.Once);
        deviceService.Verify(x => x.Toggle(powerDevices.First().HostName, 0), Times.Once);
    }

    [Fact]
    public async Task TogglePowerDevice_Should_Return_NotFound()
    {
        // Arrange
        Domain.Entities.PowerDevice powerDevice = null!;
        var logger = new Mock<ILogger<TogglePowerDeviceCommandHandler>>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var deviceService = new Mock<IDeviceService>();

        unitOfWork.Setup(x => x.PowerDeviceRepository.GetByIdAsync(1)).ReturnsAsync(powerDevice);
        deviceService.Setup(x => x.Toggle(It.IsAny<string>(), 0)).ReturnsAsync(Result.Ok());
        var handler = new TogglePowerDeviceCommandHandler(unitOfWork.Object, logger.Object, deviceService.Object);
        var command = new TogglePowerDeviceCommand(1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.First().Should().BeOfType<NotFoundError>();
        result.Errors.First().Message.Should().Be("PowerDevice not found");
        unitOfWork.Verify(x => x.PowerDeviceRepository.GetByIdAsync(1), Times.Once);
        logger.Verify(l =>
        l.Log(LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("PowerDevice with Id: 1 not found")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
    }

    [Fact]
    public async Task SwitchPowerDevice_Should_Return_Success()
    {
        // Arrange
        var powerDevices = MockTestdata();
        var logger = new Mock<ILogger<SwitchPowerDeviceCommandHandler>>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var deviceService = new Mock<IDeviceService>();

        unitOfWork.Setup(x => x.PowerDeviceRepository.GetByIdAsync(1)).ReturnsAsync(powerDevices.First());
        deviceService.Setup(x => x.Switch(It.IsAny<string>(), 0, true)).ReturnsAsync(Result.Ok());
        var handler = new SwitchPowerDeviceCommandHandler(unitOfWork.Object, logger.Object, deviceService.Object);
        var command = new SwitchPowerDeviceCommand(1, true);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        unitOfWork.Verify(x => x.PowerDeviceRepository.GetByIdAsync(1), Times.Once);
        deviceService.Verify(x => x.Switch(powerDevices.First().HostName, 0, true), Times.Once);
    }

    [Fact]
    public async Task SwitchPowerDevice_Should_Return_NotFound()
    {
        // Arrange
        Domain.Entities.PowerDevice powerDevice = null!;
        var logger = new Mock<ILogger<SwitchPowerDeviceCommandHandler>>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var deviceService = new Mock<IDeviceService>();

        unitOfWork.Setup(x => x.PowerDeviceRepository.GetByIdAsync(1)).ReturnsAsync(powerDevice);
        var handler = new SwitchPowerDeviceCommandHandler(unitOfWork.Object, logger.Object, deviceService.Object);
        var command = new SwitchPowerDeviceCommand(1, true);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.First().Should().BeOfType<NotFoundError>();
        result.Errors.First().Message.Should().Be("PowerDevice not found");
        unitOfWork.Verify(x => x.PowerDeviceRepository.GetByIdAsync(1), Times.Once);
        logger.Verify(l =>
        l.Log(LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("PowerDevice with Id: 1 not found")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
    }

    [Fact]
    public async Task PowerDeviceState_Should_Return_Success()
    {
        // Arrange
        var powerDevices = MockTestdata();
        var logger = new Mock<ILogger<GetPowerDeviceStateQueryHandler>>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var deviceService = new Mock<IDeviceService>();

        unitOfWork.Setup(x => x.PowerDeviceRepository.GetByIdAsync(1)).ReturnsAsync(powerDevices.First());
        deviceService.Setup(x => x.State(It.IsAny<string>(), 0)).ReturnsAsync(Result.Ok(true));
        var handler = new GetPowerDeviceStateQueryHandler(unitOfWork.Object, logger.Object, deviceService.Object);
        var command = new GetPowerDeviceStateQuery(1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
        unitOfWork.Verify(x => x.PowerDeviceRepository.GetByIdAsync(1), Times.Once);
        deviceService.Verify(x => x.State(powerDevices.First().HostName, 0), Times.Once);
    }

    [Fact]
    public async Task PowerDeviceState_Should_Return_NotFound()
    {
        // Arrange
        Domain.Entities.PowerDevice powerDevice = null!;
        var logger = new Mock<ILogger<GetPowerDeviceStateQueryHandler>>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var deviceService = new Mock<IDeviceService>();

        unitOfWork.Setup(x => x.PowerDeviceRepository.GetByIdAsync(1)).ReturnsAsync(powerDevice);
        var handler = new GetPowerDeviceStateQueryHandler(unitOfWork.Object, logger.Object, deviceService.Object);
        var command = new GetPowerDeviceStateQuery(1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.First().Should().BeOfType<NotFoundError>();
        result.Errors.First().Message.Should().Be("PowerDevice not found");
        unitOfWork.Verify(x => x.PowerDeviceRepository.GetByIdAsync(1), Times.Once);
        logger.Verify(l =>
        l.Log(LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("PowerDevice with Id: 1 not found")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
    }

    [Fact]
    public async Task PowerDeviceWebhook_Should_Return_Success()
    {
        // Arrange
        var powerDevices = MockTestdata();
        var logger = new Mock<ILogger<PowerDeviceWebhookQueryHandler>>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var iPublisher = new Mock<IPublisher>();
        unitOfWork.Setup(x => x.PowerDeviceRepository.GetBy(It.IsAny<Expression<Func<Askstatus.Domain.Entities.PowerDevice, bool>>>())).ReturnsAsync(powerDevices.First());
        var handler = new PowerDeviceWebhookQueryHandler(unitOfWork.Object, logger.Object, iPublisher.Object);
        var command = new PowerDeviceWebhookQuery(powerDevices.First().DeviceMac, true);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        unitOfWork.Verify(x => x.PowerDeviceRepository.GetBy(It.IsAny<Expression<Func<Askstatus.Domain.Entities.PowerDevice, bool>>>()), Times.Once);
    }

    [Fact]
    public async Task PowerDeviceWebhook_Should_Return_NotFound()
    {
        // Arrange
        Askstatus.Domain.Entities.PowerDevice powerDevice = null!;
        var logger = new Mock<ILogger<PowerDeviceWebhookQueryHandler>>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var iPublisher = new Mock<IPublisher>();
        unitOfWork.Setup(x => x.PowerDeviceRepository.GetBy(It.IsAny<Expression<Func<Askstatus.Domain.Entities.PowerDevice, bool>>>())).ReturnsAsync(powerDevice);
        var handler = new PowerDeviceWebhookQueryHandler(unitOfWork.Object, logger.Object, iPublisher.Object);
        var command = new PowerDeviceWebhookQuery("NOMAC", true);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.First().Should().BeOfType<NotFoundError>();
        result.Errors.First().Message.Should().Be("PowerDevice not found");
        unitOfWork.Verify(x => x.PowerDeviceRepository.GetBy(It.IsAny<Expression<Func<Askstatus.Domain.Entities.PowerDevice, bool>>>()), Times.Once);
        logger.Verify(l =>
        l.Log(LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("PowerDevice with mac NOMAC not found")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
    }

    [Fact]
    public async Task GetPowerDeviceBy_Should_Return_Success()
    {
        // Arrange
        var powerDevices = MockTestdata();
        var logger = new Mock<ILogger<GetPowerDeviceByMacQueryHandler>>();
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(x => x.PowerDeviceRepository.GetBy(It.IsAny<Expression<Func<Askstatus.Domain.Entities.PowerDevice, bool>>>())).ReturnsAsync(powerDevices.First());
        var handler = new GetPowerDeviceByMacQueryHandler(unitOfWork.Object, logger.Object);
        var command = new GetPowerDeviceByMacQuery(powerDevices.First().DeviceMac);

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
        unitOfWork.Verify(x => x.PowerDeviceRepository.GetBy(It.IsAny<Expression<Func<Askstatus.Domain.Entities.PowerDevice, bool>>>()), Times.Once);
    }

    [Fact]
    public async Task GetPowerDeviceBy_Should_Return_NotFound()
    {
        // Arrange
        Askstatus.Domain.Entities.PowerDevice powerDevice = null!;
        var logger = new Mock<ILogger<GetPowerDeviceByMacQueryHandler>>();
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(x => x.PowerDeviceRepository.GetBy(It.IsAny<Expression<Func<Askstatus.Domain.Entities.PowerDevice, bool>>>())).ReturnsAsync(powerDevice);
        var handler = new GetPowerDeviceByMacQueryHandler(unitOfWork.Object, logger.Object);
        var command = new GetPowerDeviceByMacQuery("NOMAC");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.First().Should().BeOfType<NotFoundError>();
        result.Errors.First().Message.Should().Be("PowerDevice not found");
        unitOfWork.Verify(x => x.PowerDeviceRepository.GetBy(It.IsAny<Expression<Func<Askstatus.Domain.Entities.PowerDevice, bool>>>()), Times.Once);
        logger.Verify(l =>
        l.Log(LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("PowerDevice with mac NOMAC not found")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
    }

    private List<Askstatus.Domain.Entities.PowerDevice> MockTestdata()
    {
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
        return new List<Askstatus.Domain.Entities.PowerDevice> { powerDevice1, poweDevice2 };
    }
}
