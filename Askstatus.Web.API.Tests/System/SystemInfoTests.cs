using System.Net;
using Askstatus.Common.Authorization;
using Askstatus.Common.Identity;
using Askstatus.Sdk;
using Askstatus.Sdk.Identity;
using Askstatus.Sdk.System;
using FluentAssertions;

namespace Askstatus.Web.API.Tests.System;

[Collection(DatabaseCollection.WebAPICollectionDefinition)]
public class SystemInfoTests
{
    private readonly HttpClient _client;
    private readonly ISystemAPI _systemApi;
    private readonly IIdentityApi _identityApi;
    private readonly IntegrationTestWebAppFactory _factory;

    public SystemInfoTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
        var askstatusApiService = new AskstatusApiService(_client);
        _systemApi = askstatusApiService.SystemAPI;
        _identityApi = askstatusApiService.IdentityApi;
        _factory = factory;
    }

    [Fact]
    public async Task GetSystemInfo()
    {
        // Arrange
        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.System);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        // Act
        var response = await _systemApi.GetSystemInfo();

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Should().NotBeNull();
        response.Content!.MailSettings.Should().NotBeNull();
        response.Content!.ApiSettings.Should().NotBeNull();
    }
}
