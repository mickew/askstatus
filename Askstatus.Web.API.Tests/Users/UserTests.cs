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
public class UserTests
{
    private readonly IIdentityApi _identityApi;
    private readonly IUserAPI _userAPI;
    private readonly HttpClient _client;
    private readonly IntegrationTestWebAppFactory _factory;

    public UserTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
        var askstatusApiService = new AskstatusApiService(_client);
        _identityApi = askstatusApiService.IdentityApi;
        _userAPI = askstatusApiService.UserAPI;
        _factory = factory;
    }

    [Fact]
    public async Task GetUsers_Should_Return_Success()
    {
        //Aranage
        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.ViewUsers);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        //Act
        var response = await _userAPI.GetUsers();

        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetUsers_Should_Return_Unauthorized()
    {
        //Aranage
        _factory.ReSeedData();
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        //Act
        var response = await _userAPI.GetUsers();

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetUserById_Should_Return_Success()
    {
        //Aranage
        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.ViewUsers);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        //Act
        var response = await _userAPI.GetUserById(_factory.AdminId!);

        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetUserById_Should_Return_NotFound()
    {
        //Aranage
        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.ViewUsers);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        //Act
        var response = await _userAPI.GetUserById("noid");

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetUserById_Should_Return_Unauthorized()
    {
        //Aranage
        _factory.ReSeedData();
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        //Act
        var response = await _userAPI.GetUserById(_factory.AdminId!);

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateUser_Should_Return_Success()
    {
        //Aranage
        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.ManageUsers);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));
        var roles = new List<string> { IntegrationTestWebAppFactory.UserRole };

        //Act
        var response = await _userAPI.CreateUser(new UserRequest("", "testuser", "testuser@localhost.local", "TestUser", "User", roles));

        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateUser_Should_Return_BadRequest()
    {
        //Aranage
        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.ManageUsers);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));
        var roles = new List<string> { IntegrationTestWebAppFactory.AdministratorsRole };

        //Act
        var response = await _userAPI.CreateUser(new UserRequest("", "admin", "admin@localhost.local", "Admin", "User", roles));

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateUser_Should_Return_Unauthorized()
    {
        //Aranage
        _factory.ReSeedData();
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));
        var roles = new List<string> { IntegrationTestWebAppFactory.UserRole };

        //Act
        var response = await _userAPI.CreateUser(new UserRequest("", "testuser", "testuser@localhost.local", "TestUser", "User", roles));

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateUser_Should_Return_Success()
    {
        //Aranage
        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.ManageUsers);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));
        var roles = new List<string> { IntegrationTestWebAppFactory.UserRole };

        //Act
        var response = await _userAPI.UpdateUser(new UserRequest(_factory.UserId!, "testuser", "testuser@localhost.local", "TestUser", "User", roles));

        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateUser_Should_Return_CantUpdateAdmin_BadRequest()
    {
        //Aranage
        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.ManageUsers);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));
        var roles = new List<string> { IntegrationTestWebAppFactory.UserRole };

        //Act
        var response = await _userAPI.UpdateUser(new UserRequest(_factory.AdminId!, "testuser", "testuser@localhost.local", "TestUser", "User", roles));

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateUser_Should_Return_BadRequest()
    {
        //Aranage
        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.ManageUsers);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));
        var roles = new List<string> { IntegrationTestWebAppFactory.UserRole };

        //Act
        var response = await _userAPI.UpdateUser(new UserRequest(_factory.UserId!, "admin", "admin@localhost.local", "Admin", "User", roles));

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    //[Fact]
    //public async Task UpdateUser_WithBadRole_Should_Return_BadRequest()
    //{
    //    //Aranage
    //    _factory.ReSeedData();
    //    await _factory.SetUsersPermission(Permissions.ManageUsers);
    //    await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));
    //    var roles = new List<string> { "NoRole" };

    //    //Act
    //    var response = await _userAPI.UpdateUser(new UserRequest(_factory.UserId!, "testuser", "testuser@localhost.local", "Testuser", "User", roles));

    //    //Assert
    //    response.IsSuccessStatusCode.Should().BeFalse();
    //    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    //}

    [Fact]
    public async Task UpdateUser_Should_Return_Unauthorized()
    {
        //Aranage
        _factory.ReSeedData();
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));
        var roles = new List<string> { IntegrationTestWebAppFactory.UserRole };

        //Act
        var response = await _userAPI.UpdateUser(new UserRequest(_factory.UserId!, "testuser", "testuser@localhost.local", "TestUser", "User", roles));

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteUser_Should_Return_Success()
    {
        //Aranage
        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.ManageUsers);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        //Act
        var response = await _userAPI.DeleteUser(_factory.UserId!);

        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteUser_Should_Return_NotFound()
    {
        //Aranage
        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.ManageUsers);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        //Act
        var response = await _userAPI.DeleteUser("noid");

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteUser_Should_Return_CantDeleteAdmin_BadRequest()
    {
        //Aranage
        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.ManageUsers);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        //Act
        var response = await _userAPI.DeleteUser(_factory.AdminId!);

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteUser_Should_Return_Unauthorized()
    {
        //Aranage
        _factory.ReSeedData();
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        //Act
        var response = await _userAPI.DeleteUser(_factory.UserId!);

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ResetPassword_Should_Return_Success()
    {
        //Aranage
        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.ManageUsers);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        //Act
        var response = await _userAPI.ResetPassword(_factory.UserId!);

        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ResetPassword_Should_Return_CantResetPassworForAdmin_BadRequest()
    {
        //Aranage
        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.ManageUsers);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        //Act
        var response = await _userAPI.ResetPassword(_factory.AdminId!);

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ResetPassword_Should_Return_Unauthorized()
    {
        //Aranage
        _factory.ReSeedData();
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        //Act
        var response = await _userAPI.DeleteUser(_factory.UserId!);

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ChangePassword_Should_Return_Success()
    {
        //Aranage
        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.ManageUsers);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        //Act
        var response = await _userAPI.ChangePassword(new ChangePasswordRequest(IntegrationTestWebAppFactory.DefaultPassword, IntegrationTestWebAppFactory.DefaultPassword, IntegrationTestWebAppFactory.DefaultPassword));

        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ChangePassword_Should_Return_BadRequest()
    {
        //Aranage
        _factory.ReSeedData();
        await _factory.SetUsersPermission(Permissions.ManageUsers);
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultUserUserName, IntegrationTestWebAppFactory.DefaultPassword));

        //Act
        var response = await _userAPI.ChangePassword(new ChangePasswordRequest(IntegrationTestWebAppFactory.DefaultPassword, "nopassword", "nopassword"));

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ChangePassword_Should_Return_CantChangePasswordForAdmin_BadRequest()
    {
        //Aranage
        _factory.ReSeedData();
        await _identityApi.Login(new LoginRequest(IntegrationTestWebAppFactory.DefaultAdminUserName, IntegrationTestWebAppFactory.DefaultPassword));

        //Act
        var response = await _userAPI.ChangePassword(new ChangePasswordRequest(IntegrationTestWebAppFactory.DefaultPassword, IntegrationTestWebAppFactory.DefaultPassword, IntegrationTestWebAppFactory.DefaultPassword));

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ChangePassword_Should_Return_Unauthorized()
    {
        //Aranage
        _factory.ReSeedData();

        //Act
        var response = await _userAPI.ChangePassword(new ChangePasswordRequest(IntegrationTestWebAppFactory.DefaultPassword, IntegrationTestWebAppFactory.DefaultPassword, IntegrationTestWebAppFactory.DefaultPassword));

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact(Skip = "cant get token from server")]
    public async Task ConfirmEmail_Should_Return_Success()
    {
        //Aranage
        _factory.ReSeedData();

        //Act
        var response = await _userAPI.ConfirmEmail(new ConfirmEmailRequest(_factory.UserId!, "token"));

        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }


    [Fact]
    public async Task ConfirmEmail_Should_Return_NotFoundFailiur()
    {
        //Aranage
        _factory.ReSeedData();

        //Act
        var response = await _userAPI.ConfirmEmail(new ConfirmEmailRequest("nouserid", "token"));

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ForgotPassword_Should_Return_Success()
    {
        //Aranage
        _factory.ReSeedData();

        //Act
        var response = await _userAPI.ForgotPassword(new ForgotPasswordRquest("user@localhost.local"));

        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ForgotPassword_Should_Return_NotFoundFailiur()
    {
        //Aranage
        _factory.ReSeedData();

        //Act
        var response = await _userAPI.ForgotPassword(new ForgotPasswordRquest("nouser@localhost.local"));

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact(Skip = "cant get token from server")]
    public async Task ResetUserPassword_Should_Return_Success()
    {
        //Aranage
        _factory.ReSeedData();

        //Act
        var response = await _userAPI.ResetUserPassword(new ResetPasswordRequest(_factory.UserId!, "token", "!Password1"));

        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ResetUserPassword_Should_Return_NotFoundFailiur()
    {
        //Aranage
        _factory.ReSeedData();

        //Act
        var response = await _userAPI.ResetUserPassword(new ResetPasswordRequest("nouserid", "token", "!Password1"));

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ResetUserPassword_Should_Return_BadRequestFailiur()
    {
        //Aranage
        _factory.ReSeedData();

        //Act
        var response = await _userAPI.ResetUserPassword(new ResetPasswordRequest(_factory.UserId!, "token", "!Password1"));

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
