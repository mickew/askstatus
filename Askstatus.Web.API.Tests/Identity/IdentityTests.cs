using System.Net;
using Askstatus.Common.Identity;
using Askstatus.Sdk;
using Askstatus.Sdk.Identity;
using FluentAssertions;

namespace Askstatus.Web.API.Tests.Identity;

[Collection(DatabaseCollection.WebAPICollectionDefinition)]
public class IdentityTests
{
    private readonly IIdentityApi _identityApi;
    private readonly HttpClient _client;
    private readonly IntegrationTestWebAppFactory _factory;

    public IdentityTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
        var askstatusApiService = new AskstatusApiService(_client);
        _identityApi = askstatusApiService.IdentityApi;
        _factory = factory;
    }

    [Fact]
    public async Task Login_Should_Return_Success()
    {
        //Aranage
        _factory.ReSeedData();

        //Act
        var response = await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultAdminUserName, IntegrationTestWebAppFactory.DefaultPassword));

        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Login_Should_Return_Unauthorized()
    {
        //Aranage
        _factory.ReSeedData();

        //Act
        var response = await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultAdminUserName, "wrongpassword"));

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Logout_Should_Return_Success()
    {
        //Aranage
        _factory.ReSeedData();
        var r = await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultAdminUserName, IntegrationTestWebAppFactory.DefaultPassword));

        //Act
        var response = await _identityApi.Logout();

        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Logout_Should_Return_Unauthorized()
    {
        //Aranage
        _factory.ReSeedData();

        //Act
        var response = await _identityApi.Logout();

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetUserInfo_Should_Return_Success()
    {
        //Aranage
        _factory.ReSeedData();
        var r = await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultAdminUserName, IntegrationTestWebAppFactory.DefaultPassword));

        //Act
        var response = await _identityApi.GetUserInfo();

        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Should().NotBeNull();
        response.Content!.Id.Should().NotBeEmpty();
        response.Content!.UserName.Should().NotBeEmpty();
        response.Content!.Email.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetUserInfo_Should_Return_Unauthorized()
    {
        //Aranage
        _factory.ReSeedData();
        var r = await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultAdminUserName, "wrongpassword"));

        //Act
        var response = await _identityApi.GetUserInfo();

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetApplicationClaims_Should_Return_Success()
    {
        //Aranage
        _factory.ReSeedData();
        var r = await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultAdminUserName, IntegrationTestWebAppFactory.DefaultPassword));

        //Act
        var response = await _identityApi.GetApplicationClaims();

        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Should().NotBeNull();
        response.Content!.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetApplicationClaims_Should_Return_Unauthorized()
    {
        //Aranage
        _factory.ReSeedData();
        var r = await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultAdminUserName, "wrongpassword"));

        //Act
        var response = await _identityApi.GetApplicationClaims();

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
