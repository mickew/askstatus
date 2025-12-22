using System.Net;
using Askstatus.Common.Authorization;
using Askstatus.Common.Identity;
using Askstatus.Common.System;
using Askstatus.Sdk;
using Askstatus.Sdk.Identity;
using Askstatus.Sdk.System;
using FluentAssertions;

namespace Askstatus.Web.API.Tests.System;

[Collection(DatabaseCollection.WebAPICollectionDefinition)]
public class SystemSendMailTests
{
    private readonly HttpClient _client;
    private readonly ISystemAPI _systemApi;
    private readonly IIdentityApi _identityApi;
    private readonly IntegrationTestWebAppFactory _factory;

    public SystemSendMailTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
        var askstatusApiService = new AskstatusApiService(_client);
        _systemApi = askstatusApiService.SystemAPI;
        _identityApi = askstatusApiService.IdentityApi;
        _factory = factory;
    }

    [Fact]
    public async Task SendMail_Should_Return_Success()
    {
        // Arrange
        await _factory.SetUsersPermission(Permissions.System);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        // Act
        var response = await _systemApi.SendMail(new SystemSendMailRequest("to@to.se", "Askstatus", "Header", "Subject", "Body"));

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SendMail_Should_Return_Unauthorized()
    {
        // Arrange
        await _factory.SetUsersPermission(Permissions.None);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        // Act
        var response = await _systemApi.SendMail(new SystemSendMailRequest("to@to.se", "Askstatus", "Header", "Subject", "Body"));

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
