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

    //private static readonly HttpConnectionFactory ConnectionFactory = new(Options.Create(
    //new HttpConnectionOptions
    //{
    //    DefaultTransferFormat = TransferFormat.Text,
    //    SkipNegotiation = false,
    //    //Transports = HttpTransportType.WebSockets,
    //    Headers =
    //    {
    //            ["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
    //                            "AppleWebKit/537.36 (KHTML, like Gecko) " +
    //                            "Chrome/113.0.0.0 Safari/537.36 Edg/113.0.1774.57"
    //    }
    //}),
    //NullLoggerFactory.Instance);

    //private static readonly JsonHubProtocol HubProtocol = new(
    //    Options.Create(new JsonHubProtocolOptions()
    //    {
    //        PayloadSerializerOptions = System.Text.Json.JsonSerializerOptions.Default
    //    }));

    //private static readonly UriEndPoint HubEndpoint = new(new Uri("https://localhost:7298/statushub"));

    public PowerDeviceTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
        var askstatusApiService = new AskstatusApiService(_client);
        _identityApi = askstatusApiService.IdentityApi;
        _powerDeviceAPI = askstatusApiService.PowerDeviceAPI;
        _factory = factory;
    }

    //[Fact]
    //public async Task Test_Hub()
    //{
    //    // Arrange
    //    int deviceId = 0;
    //    bool deviceOnOff = false;
    //    var connection = new HubConnection(ConnectionFactory, HubProtocol, HubEndpoint, new ServiceCollection().BuildServiceProvider(), NullLoggerFactory.Instance);

    //    connection.On<int, bool>("UpdateDeviceStatus", (id, onoff) =>
    //    {
    //        deviceId = id;
    //        deviceOnOff = onoff;

    //    });
    //    await connection.StartAsync();

    //    // Act
    //    var res =await _powerDeviceAPI.Webhook("EC626081CDF4", true);

    //    // Assert
    //    deviceId.Should().Be(1);
    //    deviceOnOff.Should().BeTrue();
    //}

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
            0,
            "Test Device",
            PowerDeviceTypes.ShellyGen2,
            "localhost",
            "Test Device",
            "Test Device",
            "00:00:00:00:00:00",
            "Test Model",
            1,
            ChanelType.Generic
        );

        // Act
        var response = await _powerDeviceAPI.CreatePowerDevice(powerDeviceRequest);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Should().NotBeNull();
        response.Content!.Id.Should().BeGreaterThan(2);
        response.Content!.Name.Should().Be("Test Device");
        response.Content!.DeviceType.Should().Be(PowerDeviceTypes.ShellyGen2);
        response.Content!.HostName.Should().Be("localhost");
        response.Content!.DeviceName.Should().Be("Test Device");
        response.Content!.DeviceId.Should().Be("Test Device");
        response.Content!.DeviceMac.Should().Be("00:00:00:00:00:00");
        response.Content!.DeviceModel.Should().Be("Test Model");
        response.Content!.Channel.Should().Be(1);
        response.Content!.ChanelType.Should().Be(ChanelType.Generic);
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
            1,
            ChanelType.Generic
        );

        // Act
        var response = await _powerDeviceAPI.CreatePowerDevice(powerDeviceRequest);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
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
            "192.168.1.85",
            "Test Device",
            "Test Device",
            "EC626081CDF4",
            "Test Model",
            0,
            ChanelType.Generic
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
            1,
            ChanelType.Generic
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
            1,
            ChanelType.Generic
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
            1,
            ChanelType.Generic
        );

        // Act
        var response = await _powerDeviceAPI.UpdatePowerDevice(powerDeviceRequest);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdatePowerDevice_Should_Return_BadRequest()
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
            1,
            ChanelType.Generic
        );
        var powerDeviceRequestToChange = new PowerDeviceRequest
        (
            default,
            "Test Device",
            PowerDeviceTypes.ShellyGen2,
            "localhost",
            "Test Device",
            "Test Device",
            "00:00:00:00:00:00",
            "Test Model",
            2,
            ChanelType.Generic
        );
        var createdResponse = await _powerDeviceAPI.CreatePowerDevice(powerDeviceRequest);
        var createdResponseToChange = await _powerDeviceAPI.CreatePowerDevice(powerDeviceRequestToChange);
        powerDeviceRequestToChange = new PowerDeviceRequest
        (
            createdResponseToChange.Content!.Id,
            "Test Device",
            PowerDeviceTypes.ShellyGen2,
            "localhost",
            "Test Device",
            "Test Device",
            "00:00:00:00:00:00",
            "Test Model",
            1,
            ChanelType.Generic
        );

        // Act
        var response = await _powerDeviceAPI.UpdatePowerDevice(powerDeviceRequestToChange);

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
