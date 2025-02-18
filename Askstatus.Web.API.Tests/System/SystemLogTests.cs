using System.Net;
using Askstatus.Common.Authorization;
using Askstatus.Common.Identity;
using Askstatus.Sdk;
using Askstatus.Sdk.Identity;
using Askstatus.Sdk.System;
using FluentAssertions;

namespace Askstatus.Web.API.Tests.System;

[Collection(DatabaseCollection.WebAPICollectionDefinition)]
public class SystemLogTests
{
    private readonly HttpClient _client;
    private readonly ISystemAPI _systemApi;
    private readonly IIdentityApi _identityApi;
    private readonly IntegrationTestWebAppFactory _factory;

    public SystemLogTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
        var askstatusApiService = new AskstatusApiService(_client);
        _systemApi = askstatusApiService.SystemAPI;
        _identityApi = askstatusApiService.IdentityApi;
        _factory = factory;
    }

    [Fact]
    public async Task GetSystemInfo_Should_Return_Success()
    {
        // Araange
        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.System);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        // Act
        var response = await _systemApi.GetSystemInfo("", "Id", 1, 10, false);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Should().NotBeNull();
        response.Content!.Page.Should().Be(1);
        response.Content!.PageSize.Should().Be(10);
        response.Content!.TotalCount.Should().Be(1);
        response.Content!.Data.Should().NotBeEmpty();
        response.Content!.Data.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetSystemInfoWithSearchTerm_Should_Return_Success()
    {
        // Araange
        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.System);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        // Act
        var response = await _systemApi.GetSystemInfo("notindb", "Id", 1, 10, false);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Should().NotBeNull();
        response.Content!.Page.Should().Be(1);
        response.Content!.PageSize.Should().Be(10);
        response.Content!.TotalCount.Should().Be(0);
        response.Content!.Data.Should().BeEmpty();
        response.Content!.Data.Should().HaveCount(0);
    }

    [Fact]
    public async Task GetSystemInfo_Should_Return_Unauthorized()
    {
        // Araange
        _factory.ReSeedData();
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        // Act
        var response = await _systemApi.GetSystemInfo("", "Id", 1, 10, false);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
