using System.Net;
using System.Text;
using Askstatus.Common.Authorization;
using Askstatus.Common.Identity;
using Askstatus.Sdk;
using Askstatus.Sdk.Identity;
using Askstatus.Sdk.System;
using FluentAssertions;
using Refit;

namespace Askstatus.Web.API.Tests.System;

[Collection(DatabaseCollection.WebAPICollectionDefinition)]
public class SystemUploadFileTest
{
    private readonly HttpClient _client;
    private readonly ISystemAPI _systemApi;
    private readonly IIdentityApi _identityApi;
    private readonly IntegrationTestWebAppFactory _factory;

    public SystemUploadFileTest(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
        var askstatusApiService = new AskstatusApiService(_client);
        _systemApi = askstatusApiService.SystemAPI;
        _identityApi = askstatusApiService.IdentityApi;
        _factory = factory;
    }

    [Fact]
    public async Task UploadGoogleTokenResponseFile_Should_Return_Success()
    {
        // Arrange
        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.System);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        // Act
        var response = await _systemApi.UploadGoogleTokenResponseFile(new StreamPart(new MemoryStream(Encoding.UTF8.GetBytes("test")), "test.txt"));

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UploadGoogleTokenResponseFile_Should_Return_Unauthorized()
    {
        // Arrange
        _factory.ReSeedData();
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        // Act
        var response = await _systemApi.UploadGoogleTokenResponseFile(new StreamPart(new MemoryStream(Encoding.UTF8.GetBytes("test")), "test.txt"));

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UploadProductionAppSettingsFile_Should_Return_Success()
    {
        // Arrange
        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.System);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        // Act
        var response = await _systemApi.UploadProductionAppSettingsFile(new StreamPart(new MemoryStream(Encoding.UTF8.GetBytes("test")), "appsettings.Production.json"));

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UploadProductionAppSettingsFile_Should_Return_Unauthorized()
    {
        // Arrange
        _factory.ReSeedData();
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        // Act
        var response = await _systemApi.UploadProductionAppSettingsFile(new StreamPart(new MemoryStream(Encoding.UTF8.GetBytes("test")), "appsettings.Production.json"));

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
