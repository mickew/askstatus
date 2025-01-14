using System.Net;
using Askstatus.Common.Authorization;
using Askstatus.Common.Identity;
using Askstatus.Common.PowerDevice;
using Askstatus.Sdk;
using Askstatus.Sdk.Identity;
using Askstatus.Sdk.PowerDevices;
using FluentAssertions;

namespace Askstatus.Web.API.Tests.PowerDevices;

[Collection(DatabaseCollection.WebAPICollectionDefinition)]
public class PowerDeviceTests
{
    private readonly HttpClient _client;
    private readonly IntegrationTestWebAppFactory _factory;
    private readonly IIdentityApi _identityApi;
    private readonly IPowerDeviceAPI _powerDeviceAPI;

    public PowerDeviceTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
        var askstatusApiService = new AskstatusApiService(_client);
        _identityApi = askstatusApiService.IdentityApi;
        _powerDeviceAPI = askstatusApiService.PowerDeviceAPI;
        _factory = factory;
    }

    [Fact]
    public async Task GetPowerDevices_Should_Return_Success()
    {
        // Arrange
        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.ViewPowerDevices);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        // Act
        var response = await _powerDeviceAPI.GetPowerDevices();

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetPowerDevices_Should_Return_Unauthorized()
    {
        // Arrange
        _factory.ReSeedData();
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        // Act
        var response = await _powerDeviceAPI.GetPowerDevices();

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetPowerDeviceById_Should_Return_Success()
    {
        // Arrange
        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.ViewPowerDevices);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        // Act
        var response = await _powerDeviceAPI.GetPowerDeviceById(_factory.PowerDeviceId);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetPowerDeviceById_Should_Return_Unauthorized()
    {
        // Arrange
        _factory.ReSeedData();
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        // Act
        var response = await _powerDeviceAPI.GetPowerDeviceById(_factory.PowerDeviceId);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetPowerDeviceById_Should_Return_NotFound()
    {
        // Arrange
        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.ViewPowerDevices);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        // Act
        var response = await _powerDeviceAPI.GetPowerDeviceById(-1);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreatePowerDevice_Should_Return_Success()
    {
        // Arrange
        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.ConfigurePowerDevices);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));
        var powerDeviceRequest = new PowerDeviceRequest
        (
            default,
            "Test Device",
            PowerDeviceTypes.ShellyGen2,
            "localhost",
            "Test Device",
            "Test Device",
            "00:00:00:00:00:00",
            "Test Model",
            1
        );

        // Act
        var response = await _powerDeviceAPI.CreatePowerDevice(powerDeviceRequest);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreatePowerDevice_Should_Return_Unauthorized()
    {
        // Arrange
        _factory.ReSeedData();
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));
        var powerDeviceRequest = new PowerDeviceRequest
        (
            default,
            "Test Device",
            PowerDeviceTypes.ShellyGen2,
            "localhost",
            "Test Device",
            "Test Device",
            "00:00:00:00:00:00",
            "Test Model",
            1
        );

        // Act
        var response = await _powerDeviceAPI.CreatePowerDevice(powerDeviceRequest);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact(Skip = "in-memory database cant test indexes")]
    public async Task CreatePowerDevice_Should_Return_BadRequest()
    {
        // Arrange
        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.ConfigurePowerDevices);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));
        var powerDeviceRequest = new PowerDeviceRequest
        (
            default,
            "Test Device",
            PowerDeviceTypes.ShellyGen2,
            "localhost",
            "Test Device",
            "Test Device",
            "00:00:00:00:00:00",
            "Test Model",
            1
        );

        // Act
        var response = await _powerDeviceAPI.CreatePowerDevice(powerDeviceRequest);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdatePowerDevice_Should_Return_Success()
    {
        // Arrange
        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.ConfigurePowerDevices);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));
        var powerDeviceRequest = new PowerDeviceRequest
        (
            _factory.PowerDeviceId,
            "Test Device",
            PowerDeviceTypes.ShellyGen2,
            "localhost",
            "Test Device",
            "Test Device",
            "00:00:00:00:00:00",
            "Test Model",
            1
        );

        // Act
        var response = await _powerDeviceAPI.UpdatePowerDevice(powerDeviceRequest);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdatePowerDevice_Should_Return_Unauthorized()
    {
        // Arrange
        _factory.ReSeedData();
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));
        var powerDeviceRequest = new PowerDeviceRequest
        (
            _factory.PowerDeviceId,
            "Test Device",
            PowerDeviceTypes.ShellyGen2,
            "localhost",
            "Test Device",
            "Test Device",
            "00:00:00:00:00:00",
            "Test Model",
            1
        );

        // Act
        var response = await _powerDeviceAPI.UpdatePowerDevice(powerDeviceRequest);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdatePowerDevice_Should_Return_NotFound()
    {
        // Arrange
        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.ConfigurePowerDevices);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));
        var powerDeviceRequest = new PowerDeviceRequest
        (
            -1,
            "Test Device",
            PowerDeviceTypes.ShellyGen2,
            "localhost",
            "Test Device",
            "Test Device",
            "00:00:00:00:00:00",
            "Test Model",
            1
        );

        // Act
        var response = await _powerDeviceAPI.UpdatePowerDevice(powerDeviceRequest);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact(Skip = "in-memory database cant test indexes")]
    public async Task UpdatePowerDevice_Should_Return_BadRequest()
    {
        // Arrange
        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.ConfigurePowerDevices);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));
        var powerDeviceRequest = new PowerDeviceRequest
        (
            -1,
            "Test Device",
            PowerDeviceTypes.ShellyGen2,
            "localhost",
            "Test Device",
            "Test Device",
            "00:00:00:00:00:00",
            "Test Model",
            1
        );

        // Act
        var response = await _powerDeviceAPI.UpdatePowerDevice(powerDeviceRequest);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeletePowerDevice_Should_Return_Success()
    {
        // Arrange
        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.ConfigurePowerDevices);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        // Act
        var response = await _powerDeviceAPI.DeletePowerDevice(_factory.PowerDeviceId);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeletePowerDevice_Should_Return_Unauthorized()
    {
        // Arrange
        _factory.ReSeedData();
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        // Act
        var response = await _powerDeviceAPI.DeletePowerDevice(_factory.PowerDeviceId);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeletePowerDevice_Should_Return_NotFound()
    {
        // Arrange
        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.ConfigurePowerDevices);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        // Act
        var response = await _powerDeviceAPI.DeletePowerDevice(-1);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact(Skip = "Cant produse BadRequest")]
    public async Task DeletePowerDevice_Should_Return_BadRequest()
    {
        // Arrange
        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.ConfigurePowerDevices);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        // Act
        var response = await _powerDeviceAPI.DeletePowerDevice(_factory.PowerDeviceId);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
