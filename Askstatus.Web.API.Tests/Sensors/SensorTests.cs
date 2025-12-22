using System.Net;
using Askstatus.Common.Authorization;
using Askstatus.Common.Identity;
using Askstatus.Common.Sensor;
using Askstatus.Sdk;
using Askstatus.Sdk.Identity;
using Askstatus.Sdk.Sensors;
using FluentAssertions;

namespace Askstatus.Web.API.Tests.Sensors;

[Collection(DatabaseCollection.WebAPICollectionDefinition)]
public class SensorTests
{
    private readonly HttpClient _client;
    private readonly IntegrationTestWebAppFactory _factory;
    private readonly IIdentityApi _identityApi;
    private readonly ISensorDiscoverAPI _sensorDiscoverAPI;
    private readonly ISensorAPI _sensorAPI;

    public SensorTests(IntegrationTestWebAppFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        var askstatusApiService = new AskstatusApiService(_client);
        _identityApi = askstatusApiService.IdentityApi;
        _sensorDiscoverAPI = askstatusApiService.SensorDiscoverAPI;
        _sensorAPI = askstatusApiService.SensorAPI;
    }

    [Fact]
    public async Task GetAllDiscoverdSensors_ShouldReturnOk()
    {
        // Arrange
        await ArrangeSensors();

        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.DiscoverSensors);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));


        // Act
        var response = await _sensorDiscoverAPI.DiscoverAll();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var sensors = response.Content;
        sensors.Should().NotBeNull();
        sensors!.Should().HaveCount(1);
        sensors.First().Values.Should().HaveCount(2);

    }
    [Fact]
    public async Task GetAllDiscoverdSensors_ShouldReturnUnauthorized()
    {
        // Arrange
        await ArrangeSensors();

        // Act
        var response = await _sensorDiscoverAPI.DiscoverAll();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetDiscoverdSensors_ShouldReturnOk()
    {
        // Arrange
        await ArrangeSensors();

        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.DiscoverSensors);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        // Act
        var response = await _sensorDiscoverAPI.NotAssigned();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var sensors = response.Content;
        sensors.Should().NotBeNull();
        sensors!.Should().HaveCount(1);
        sensors.First().Values.Should().HaveCount(1);

    }
    [Fact]
    public async Task GetDiscoverdSensors_ShouldReturnUnauthorized()
    {
        // Arrange
        await ArrangeSensors();

        // Act
        var response = await _sensorDiscoverAPI.NotAssigned();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetSensors_Should_Return_Success()
    {
        // Arrange
        await ArrangeSensors();

        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.None);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        // Act
        var response = await _sensorAPI.GetSensors();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var sensors = response.Content;
        sensors.Should().NotBeNull();
        sensors!.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetSensors_Should_Return_Unauthorized()
    {
        // Arrange
        await ArrangeSensors();

        _factory.ReSeedData();

        // Act
        var response = await _sensorAPI.GetSensors();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetSensorById_Should_Return_Success()
    {
        // Arrange
        await ArrangeSensors();

        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.ViewSensors);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        // Act
        var response = await _sensorAPI.GetSensorById(_factory.SensorId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetSensorById_Should_Return_Unauthorized()
    {
        // Arrange
        await ArrangeSensors();

        _factory.ReSeedData();

        // Act
        var response = await _sensorAPI.GetSensorById(_factory.SensorId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetSensorById_Should_Return_NotFound()
    {
        // Arrange
        await ArrangeSensors();

        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.ViewSensors);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        // Act
        var response = await _sensorAPI.GetSensorById(0);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateSensor_Should_Return_Success()
    {
        // Arrange
        await ArrangeSensors();

        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.ConfigureSensors);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));
        var sensorRequest = new SensorRequest
        (
            0,
            "Test Sensor",
            SensorType.Temperature,
            "{0} %",
            "shellyht-CC2D5C",
            "SHHT-1",
            "humidity"
        );

        // Act
        var response = await _sensorAPI.CreateSensor(sensorRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Should().NotBeNull();
        response.Content!.Id.Should().BeGreaterThanOrEqualTo(_factory.SensorId + 1);
        response.Content!.Name.Should().Be("Test Sensor");
        response.Content!.SensorType.Should().Be(SensorType.Temperature);
        response.Content!.FormatString.Should().Be("{0} %");
        response.Content!.SensorName.Should().Be("shellyht-CC2D5C");
        response.Content!.SensorModel.Should().Be("SHHT-1");
        response.Content!.ValueName.Should().Be("humidity");
    }

    [Fact]
    public async Task CreateSensor_Should_Return_Unauthorized()
    {
        // Arrange
        await ArrangeSensors();

        _factory.ReSeedData();
        var sensorRequest = new SensorRequest
        (
            0,
            "Test Sensor",
            SensorType.Temperature,
            "{0} %",
            "shellyht-CC2D5C",
            "SHHT-1",
            "humidity"
        );

        // Act
        var response = await _sensorAPI.CreateSensor(sensorRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateSensor_Should_Return_BadRequest()
    {
        // Arrange
        await ArrangeSensors();

        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.ConfigureSensors);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));
        var sensorRequest = new SensorRequest
        (
            0,
            "Test Sensor",
            SensorType.Temperature,
            "{0} °C",
            "shellyht-CC2D5C",
            "SHHT-1",
            "temperature"
        );

        // Act
        var response = await _sensorAPI.CreateSensor(sensorRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateSensor_Should_Return_Success()
    {
        // Arrange
        _factory.ReSeedData();
        await ArrangeSensors();

        await _factory.SetUsersPermission(Permissions.ConfigureSensors);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));
        var sensorRequest = new SensorRequest
        (
            _factory.SensorId,
            "Test Sensor",
            SensorType.Temperature,
            "0.00 °C",
            "shellyht-CC2D5C",
            "SHHT-1",
            "temperature"
        );

        // Act
        var response = await _sensorAPI.UpdateSensor(sensorRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateSensor_Should_Return_Unauthorized()
    {
        // Arrange
        _factory.ReSeedData();
        await ArrangeSensors();

        var sensorRequest = new SensorRequest
        (
            _factory.SensorId,
            "Test Sensor",
            SensorType.Temperature,
            "0.00 °C",
            "shellyht-CC2D5C",
            "SHHT-1",
            "temperature"
        );

        // Act
        var response = await _sensorAPI.UpdateSensor(sensorRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateSensor_Should_Return_NotFound()
    {
        // Arrange
        _factory.ReSeedData();
        await ArrangeSensors();

        await _factory.SetUsersPermission(Permissions.ConfigureSensors);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));
        var sensorRequest = new SensorRequest
        (
            -1,
            "Test Sensor",
            SensorType.Temperature,
            "0.00 °C",
            "shellyht-CC2D5C",
            "SHHT-1",
            "temperature"
        );

        // Act
        var response = await _sensorAPI.UpdateSensor(sensorRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateSensor_Should_Return_BadRequest()
    {
        // Arrange
        _factory.ReSeedData();
        await ArrangeSensors();

        await _factory.SetUsersPermission(Permissions.ConfigureSensors);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));
        var sensorRequestToChange = new SensorRequest
        (
            0,
            "Test Sensor",
            SensorType.Humidity,
            "{0} %",
            "shellyht-CC2D5C",
            "SHHT-1",
            "humidity"
        );
        var sensorAdded = await _sensorAPI.CreateSensor(sensorRequestToChange);

        var sensorRequest = new SensorRequest
        (
            sensorAdded.Content!.Id,
            "Test Sensor",
            SensorType.Temperature,
            "0.00 °C",
            "shellyht-CC2D5C",
            "SHHT-1",
            "temperature"
        );

        // Act
        var response = await _sensorAPI.UpdateSensor(sensorRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteSensor_Should_Return_Success()
    {
        // Arrange
        await ArrangeSensors();

        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.ConfigureSensors);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        // Act
        var response = await _sensorAPI.DeleteSensor(_factory.SensorId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteSensor_Should_Return_Unauthorized()
    {
        // Arrange
        await ArrangeSensors();

        _factory.ReSeedData();

        // Act
        var response = await _sensorAPI.DeleteSensor(_factory.SensorId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteSensor_Should_Return_NotFound()
    {
        // Arrange
        await ArrangeSensors();

        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.ConfigureSensors);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        // Act
        var response = await _sensorAPI.DeleteSensor(-1);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact(Skip = "Cant produse BadRequest")]
    public async Task DeleteSensor_Should_Return_BadRequest()
    {
        // Arrange
        await ArrangeSensors();

        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.ConfigureSensors);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        // Act
        var response = await _sensorAPI.DeleteSensor(_factory.SensorId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetSensorValues_Should_Return_Success()
    {
        // Arrange
        await ArrangeSensors();
        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.None);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));
        // Act
        var response = await _sensorAPI.GetSensorValue(_factory.SensorId);
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var value = response.Content;
        value.Should().NotBeNull();
        value.Value.Should().Be("25.5 °C");
    }

    [Fact]
    public async Task GetSensorValues_Should_Return_Unauthorized()
    {
        // Arrange
        await ArrangeSensors();
        _factory.ReSeedData();
        // Act
        var response = await _sensorAPI.GetSensorValue(_factory.SensorId);
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetSensorValues_Should_Return_NotFound()
    {
        // Arrange
        await ArrangeSensors();
        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.None);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));
        // Act
        var response = await _sensorAPI.GetSensorValue(-1);
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task ArrangeSensors()
    {
        var json = "{ \"id\":\"shellyht-CC2D5C\",\"model\":\"SHHT-1\",\"mac\":\"483FDACC2D5C\",\"ip\":\"192.168.1.55\",\"new_fw\":false,\"fw_ver\":\"20230913-112531/v1.14.0-gcb84623\"}";
        await _factory.SendMqttMessage("shellies/announce", json);
        await _factory.SendMqttMessage("shellies/shellyht-CC2D5C/sensor/temperature", "25.5");
        await _factory.SendMqttMessage("shellies/shellyht-CC2D5C/sensor/humidity", "60");
    }
}
