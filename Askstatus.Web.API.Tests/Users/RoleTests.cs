using System.Net;
using Askstatus.Common.Authorization;
using Askstatus.Common.Identity;
using Askstatus.Common.Users;
using Askstatus.Sdk;
using Askstatus.Sdk.Identity;
using Askstatus.Sdk.Users;
using FluentAssertions;

namespace Askstatus.Web.API.Tests.Users;

[Collection(DatabaseCollection.WebAPICollectionDefinition)]
public class RoleTests
{
    private readonly IIdentityApi _identityApi;
    private readonly IRoleAPI _roleAPI;
    //private readonly IUserAPI _userAPI;
    private readonly HttpClient _client;
    private readonly IntegrationTestWebAppFactory _factory;

    public RoleTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
        var askstatusApiService = new AskstatusApiService(_client);
        _identityApi = askstatusApiService.IdentityApi;
        //_userAPI = askstatusApiService.UserAPI;
        _roleAPI = askstatusApiService.RoleAPI;
        _factory = factory;
    }

    [Fact]
    public async Task GetRoles_Should_Return_Success()
    {
        // Arrange
        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.ViewRoles);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        // Act
        var response = await _roleAPI.GetRoles();

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Should().NotBeNull();
        response.Content.Should().NotBeEmpty();
        response.Content.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetRoles_Should_Return_Unauthorized()
    {
        //Aranage
        _factory.ReSeedData();
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        //Act
        var response = await _roleAPI.GetRoles();

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateRole_Should_Return_Success()
    {
        // Arrange
        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.ManageRoles);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        // Act
        var response = await _roleAPI.CreateRole(new RoleRequest(string.Empty, "TestRole", Permissions.None));

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Should().NotBeNull();
        response.Content!.Id.Should().NotBeNullOrEmpty();
        response.Content!.Name.Should().Be("TestRole");
        response.Content!.Permissions.Should().Be(Permissions.None);
    }

    [Fact]
    public async Task CreateRole_Should_Return_BadRequest()
    {
        // Arrange
        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.ManageRoles);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        // Act
        var response = await _roleAPI.CreateRole(new RoleRequest(string.Empty, IntegrationTestWebAppFactory.AdministratorsRole, Permissions.None));

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateRole_Should_Return_Unauthorized()
    {
        // Arrange
        _factory.ReSeedData();
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        // Act
        var response = await _roleAPI.CreateRole(new RoleRequest(string.Empty, "TestRole", Permissions.None));

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateRole_Should_Return_Success()
    {
        // Arrange
        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.ManageRoles);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        // Act
        var response = await _roleAPI.UpdateRole(new RoleRequest(_factory.UserRoleId!, "NewName", Permissions.None));

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateRole_Should_Return_NotFound()
    {
        // Arrange
        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.ManageRoles);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        // Act
        var response = await _roleAPI.UpdateRole(new RoleRequest("noid", "NewName", Permissions.None));

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateRole_Should_Return_CantUpdateAdminRole_BadRequest()
    {
        // Arrange
        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.ManageRoles);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        // Act
        var response = await _roleAPI.UpdateRole(new RoleRequest(_factory.AdministratorsRoleId!, "NewName", Permissions.None));

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateRole_Should_Return_BadRequest()
    {
        // Arrange
        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.ManageRoles);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        // Act
        var response = await _roleAPI.UpdateRole(new RoleRequest(_factory.UserRoleId!, IntegrationTestWebAppFactory.AdministratorsRole, Permissions.None));

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateRole_Should_Return_Unauthorized()
    {
        // Arrange
        _factory.ReSeedData();
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        // Act
        var response = await _roleAPI.UpdateRole(new RoleRequest(_factory.UserRoleId!, "newname", Permissions.None));

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteRole_Should_Return_Success()
    {
        // Arrange
        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.ManageRoles);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        // Act
        var response = await _roleAPI.DeleteRole(_factory.UserRoleId!);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteRole_Should_Return_CantDeleteAdminRole_BadRequest()
    {
        // Arrange
        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.ManageRoles);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        // Act
        var response = await _roleAPI.DeleteRole(_factory.AdministratorsRoleId!);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteRole_Should_Return_NotFound()
    {
        // Arrange
        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.ManageRoles);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        // Act
        var response = await _roleAPI.DeleteRole("noid");

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteRole_Should_Return_Unauthorized()
    {
        // Arrange
        _factory.ReSeedData();
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        // Act
        var response = await _roleAPI.DeleteRole(_factory.UserRoleId!);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetPermissions_Should_Return_Success()
    {
        // Arrange
        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.ViewRoles);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        // Act
        var response = await _roleAPI.GetPermissions();

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdatePermissions_Should_Return_Success()
    {
        // Arrange
        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.ManageRoles);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        // Act
        var response = await _roleAPI.UpdatePermissions(new RoleRequest(_factory.UserRoleId!, IntegrationTestWebAppFactory.UserRole, Permissions.All));

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdatePermissions_Should_Return_CantUpdateAdminRole_BadRequest()
    {
        // Arrange
        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.ManageRoles);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        // Act
        var response = await _roleAPI.UpdatePermissions(new RoleRequest(_factory.AdministratorsRoleId!, "new name", Permissions.None));

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdatePermissions_Should_Return_NotFound()
    {
        // Arrange
        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.ManageRoles);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        // Act
        var response = await _roleAPI.UpdatePermissions(new RoleRequest("noid", "new name", Permissions.None));

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdatePermissionsRole_Should_Return_Unauthorized()
    {
        // Arrange
        _factory.ReSeedData();
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        // Act
        var response = await _roleAPI.UpdatePermissions(new RoleRequest(_factory.UserRoleId!, IntegrationTestWebAppFactory.UserRole, Permissions.All));

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
