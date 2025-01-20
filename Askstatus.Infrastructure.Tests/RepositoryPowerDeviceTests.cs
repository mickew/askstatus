using Askstatus.Application.Interfaces;
using Askstatus.Application.PowerDevice;
using Askstatus.Common.PowerDevice;
using Askstatus.Domain.Entities;
using Askstatus.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace Askstatus.Infrastructure.Tests;
public class RepositoryPowerDeviceTests
{
    [Fact]
    public async Task PowerDeviceRepository_AddAsync_ShouldAddPowerDevice()
    {
        // Arrange
        var serviceProvider = new ServiceCollection()
            .AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()))
            .AddLogging()
            .AddScoped<IRepository<PowerDevice>, Repository<PowerDevice>>()
            .BuildServiceProvider();
        var unitOfWork = new UnitOfWork(serviceProvider.GetRequiredService<ApplicationDbContext>(), serviceProvider);
        var powerDeviceRepository = unitOfWork.PowerDeviceRepository;
        var powerDevice = new PowerDevice
        {
            Name = "PowerDevice1",
            DeviceType = PowerDeviceTypes.Generic,
            HostName = "HostName1",
            DeviceName = "DeviceName1",
            DeviceId = "DeviceId1",
            DeviceMac = "DeviceMac1",
            DeviceModel = "DeviceModel1",
            Channel = 1,
        };
        // Act
        var result = await powerDeviceRepository.AddAsync(powerDevice);
        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(powerDevice.Name);
        result.DeviceType.Should().Be(powerDevice.DeviceType);
        result.HostName.Should().Be(powerDevice.HostName);
        result.DeviceName.Should().Be(powerDevice.DeviceName);
        result.DeviceId.Should().Be(powerDevice.DeviceId);
        result.DeviceMac.Should().Be(powerDevice.DeviceMac);
        result.DeviceModel.Should().Be(powerDevice.DeviceModel);
        result.Channel.Should().Be(powerDevice.Channel);
    }

    [Fact]
    public async Task PowerDeviceRepository_GetByIdAsync_ShouldReturnPowerDevice()
    {
        // Arrange
        var serviceProvider = new ServiceCollection()
            .AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()))
            .AddLogging()
            .AddScoped<IRepository<PowerDevice>, Repository<PowerDevice>>()
            .BuildServiceProvider();
        var unitOfWork = new UnitOfWork(serviceProvider.GetRequiredService<ApplicationDbContext>(), serviceProvider);
        var powerDeviceRepository = unitOfWork.PowerDeviceRepository;
        var powerDevice = new PowerDevice
        {
            Name = "PowerDevice1",
            DeviceType = PowerDeviceTypes.Generic,
            HostName = "HostName1",
            DeviceName = "DeviceName1",
            DeviceId = "DeviceId1",
            DeviceMac = "DeviceMac1",
            DeviceModel = "DeviceModel1",
            Channel = 1,
        };
        var addedPowerDevice = await powerDeviceRepository.AddAsync(powerDevice);

        // Act
        var result = await powerDeviceRepository.GetByIdAsync(addedPowerDevice.Id);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(powerDevice.Name);
        result.DeviceType.Should().Be(powerDevice.DeviceType);
        result.HostName.Should().Be(powerDevice.HostName);
        result.DeviceName.Should().Be(powerDevice.DeviceName);
        result.DeviceId.Should().Be(powerDevice.DeviceId);
        result.DeviceMac.Should().Be(powerDevice.DeviceMac);
        result.DeviceModel.Should().Be(powerDevice.DeviceModel);
        result.Channel.Should().Be(powerDevice.Channel);
    }

    [Fact]
    public async Task PowerDeviceRepository_GetAllAsync_ShouldReturnAllPowerDevices()
    {
        // Arrange
        var serviceProvider = new ServiceCollection()
            .AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()))
            .AddLogging()
            .AddScoped<IRepository<PowerDevice>, Repository<PowerDevice>>()
            .BuildServiceProvider();
        var unitOfWork = new UnitOfWork(serviceProvider.GetRequiredService<ApplicationDbContext>(), serviceProvider);
        var powerDeviceRepository = unitOfWork.PowerDeviceRepository;
        var powerDevice1 = new PowerDevice
        {
            Name = "PowerDevice1",
            DeviceType = PowerDeviceTypes.Generic,
            HostName = "HostName1",
            DeviceName = "DeviceName1",
            DeviceId = "DeviceId1",
            DeviceMac = "DeviceMac1",
            DeviceModel = "DeviceModel1",
            Channel = 1,
        };
        var powerDevice2 = new PowerDevice
        {
            Name = "PowerDevice2",
            DeviceType = PowerDeviceTypes.Generic,
            HostName = "HostName2",
            DeviceName = "DeviceName2",
            DeviceId = "DeviceId2",
            DeviceMac = "DeviceMac2",
            DeviceModel = "DeviceModel2",
            Channel = 2,
        };
        await powerDeviceRepository.AddAsync(powerDevice1);
        await powerDeviceRepository.AddAsync(powerDevice2);
        await unitOfWork.SaveChangesAsync();

        // Act
        var result = await powerDeviceRepository.ListAllAsync();

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task PowerDeviceRepository_Update_ShouldUpdatePowerDevice()
    {
        // Arrange
        var serviceProvider = new ServiceCollection()
            .AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()))
            .AddLogging()
            .AddScoped<IRepository<PowerDevice>, Repository<PowerDevice>>()
            .BuildServiceProvider();
        var unitOfWork = new UnitOfWork(serviceProvider.GetRequiredService<ApplicationDbContext>(), serviceProvider);
        var powerDeviceRepository = unitOfWork.PowerDeviceRepository;
        var powerDevice = new PowerDevice
        {
            Name = "PowerDevice1",
            DeviceType = PowerDeviceTypes.Generic,
            HostName = "HostName1",
            DeviceName = "DeviceName1",
            DeviceId = "DeviceId1",
            DeviceMac = "DeviceMac1",
            DeviceModel = "DeviceModel1",
            Channel = 1,
        };
        var addedPowerDevice = await powerDeviceRepository.AddAsync(powerDevice);
        await unitOfWork.SaveChangesAsync();

        addedPowerDevice.Name = "PowerDevice2";
        addedPowerDevice.DeviceType = PowerDeviceTypes.ShellyGen2;
        addedPowerDevice.HostName = "HostName2";
        addedPowerDevice.DeviceName = "DeviceName2";
        addedPowerDevice.DeviceId = "DeviceId2";
        addedPowerDevice.DeviceMac = "DeviceMac2";
        addedPowerDevice.DeviceModel = "DeviceModel2";
        addedPowerDevice.Channel = 2;

        // Act
        var result = await powerDeviceRepository.UpdateAsync(addedPowerDevice);
        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task PowerDeviceRepository_Delete_ShouldDeletePowerDevice()
    {
        // Arrange
        var serviceProvider = new ServiceCollection()
            .AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()))
            .AddLogging()
            .AddScoped<IRepository<PowerDevice>, Repository<PowerDevice>>()
            .BuildServiceProvider();
        var unitOfWork = new UnitOfWork(serviceProvider.GetRequiredService<ApplicationDbContext>(), serviceProvider);
        var powerDeviceRepository = unitOfWork.PowerDeviceRepository;
        var powerDevice = new PowerDevice
        {
            Name = "PowerDevice1",
            DeviceType = PowerDeviceTypes.Generic,
            HostName = "HostName1",
            DeviceName = "DeviceName1",
            DeviceId = "DeviceId1",
            DeviceMac = "DeviceMac1",
            DeviceModel = "DeviceModel1",
            Channel = 1,
        };
        var addedPowerDevice = await powerDeviceRepository.AddAsync(powerDevice);
        await unitOfWork.SaveChangesAsync();

        // Act
        var result = await powerDeviceRepository.DeleteAsync(addedPowerDevice);

        // Assert
        result.Should().BeTrue();
    }
}
