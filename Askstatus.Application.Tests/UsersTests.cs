using Askstatus.Application.Interfaces;
using Askstatus.Application.PowerDevice;
using Askstatus.Application.Users;
using Askstatus.Common.Authorization;
using Askstatus.Common.Users;
using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace Askstatus.Application.Tests;
public class UsersTests
{
    [Fact]
    public async Task GetUsers_Should_Return_Success()
    {
        // Arrange
        Mock<IUserService> mock = new Mock<IUserService>();
        var users = new List<UserVM>
        {
            new UserVM("1", "testuser1","testuser1@local","testuser1","testuser1"),
            new UserVM("2", "testuser2","testuser2@local","testuser2","testuser2"),
        };
        mock.Setup(x => x.GetUsers()).ReturnsAsync(Result.Ok<IEnumerable<UserVM>>(users));
        GetUsersQueryHandler getUsersQueryHandler = new GetUsersQueryHandler(mock.Object);
        var getUsersQuery = new GetUsersQuery();

        // Act
        var result = await getUsersQueryHandler.Handle(getUsersQuery, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetUserById_Should_Return_Success()
    {
        // Arrange
        Mock<IUserService> mock = new Mock<IUserService>();
        var user = new UserVM("1", "testuser1", "testuser1@local", "testuser1", "testuser1");
        mock.Setup(x => x.GetUserById(It.IsAny<string>())).ReturnsAsync(Result.Ok(user));
        GetUserByIdQueryHandler getUserByIdQueryHandler = new GetUserByIdQueryHandler(mock.Object);
        var getUserByIdQuery = new GetUserByIdQuery("1");

        // Act
        var result = await getUserByIdQueryHandler.Handle(getUserByIdQuery, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be("1");
    }

    [Fact]
    public async Task GetUserById_Should_Return_Failiure()
    {
        // Arrange
        Mock<IUserService> mock = new Mock<IUserService>();
        mock.Setup(x => x.GetUserById(It.IsAny<string>())).ReturnsAsync(Result.Fail<UserVM>("User not found"));
        GetUserByIdQueryHandler getUserByIdQueryHandler = new GetUserByIdQueryHandler(mock.Object);
        var getUserByIdQuery = new GetUserByIdQuery("1");

        // Act
        var result = await getUserByIdQueryHandler.Handle(getUserByIdQuery, CancellationToken.None);

        // Assert
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Message.Should().Be("User not found");
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateUser_Should_Return_Success()
    {
        // Arrange
        Mock<IUserService> mock = new Mock<IUserService>();
        var userRequest = new UserRequest("1", "testuser1", "testuser1@local", "testuser1", "testuser1", null!);
        mock.Setup(x => x.UpdateUser(It.IsAny<UserRequest>())).ReturnsAsync(Result.Ok());
        UpdateUserCommandHandler updateUserCommandHandler = new UpdateUserCommandHandler(mock.Object);
        var updateUserCommand = new UpdateUserCommand
        {
            Id = "1",
            UserName = "testuser1",
            Email = "testuser1@local",
            FirstName = "testuser1",
            LastName = "testuser1"
        };

        // Act
        var result = await updateUserCommandHandler.Handle(updateUserCommand, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateUser_Should_Return_Failiure()
    {
        // Arrange
        Mock<IUserService> mock = new Mock<IUserService>();
        var userRequest = new UserRequest("1", "testuser1", "testuser1@local", "testuser1", "testuser1", null!);
        mock.Setup(x => x.UpdateUser(It.IsAny<UserRequest>())).ReturnsAsync(Result.Fail("User not found"));
        UpdateUserCommandHandler updateUserCommandHandler = new UpdateUserCommandHandler(mock.Object);
        var updateUserCommand = new UpdateUserCommand
        {
            Id = "1",
            UserName = "testuser1",
            Email = "testuser1@local",
            FirstName = "testuser1",
            LastName = "testuser1"
        };

        // Act
        var result = await updateUserCommandHandler.Handle(updateUserCommand, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Message.Should().Be("User not found");
    }

    [Fact]
    public async Task CreateUser_Should_Return_Success()
    {
        // Arrange
        var logger = new Mock<ILogger<CreateUserCommandHandler>>();
        var publisher = new Mock<IPublisher>();
        Mock<IUserService> mock = new Mock<IUserService>();
        var userRequest = new UserRequest("", "testuser1", "testuser1@local", "testuser1", "testuser1", null!);
        var user = new UserVMWithLink("1", "testuser1", "testuser1@local", "testuser1", "testuser1", "/link?id=1");
        mock.Setup(x => x.CreateUser(It.IsAny<UserRequest>())).ReturnsAsync(Result.Ok(user));
        CreateUserCommandHandler createUserCommandHandler = new CreateUserCommandHandler(mock.Object, logger.Object, publisher.Object);
        var createUserCommand = new CreateUserCommand
        {
            UserName = "testuser1",
            Email = "testuser1@local",
            FirstName = "testuser1",
            LastName = "testuser1"
        };

        // Act
        var result = await createUserCommandHandler.Handle(createUserCommand, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be("1");
    }

    [Fact]
    public async Task CreateUser_Should_Return_Failiure()
    {
        // Arrange
        var logger = new Mock<ILogger<CreateUserCommandHandler>>();
        var publisher = new Mock<IPublisher>();
        Mock<IUserService> mock = new Mock<IUserService>();
        var userRequest = new UserRequest("", "testuser1", "testuser1@local", "testuser1", "testuser1", null!);
        mock.Setup(x => x.CreateUser(It.IsAny<UserRequest>())).ReturnsAsync(Result.Fail("Could not create user"));
        CreateUserCommandHandler createUserCommandHandler = new CreateUserCommandHandler(mock.Object, logger.Object, publisher.Object);
        var createUserCommand = new CreateUserCommand
        {
            UserName = "testuser1",
            Email = "testuser1@local",
            FirstName = "testuser1",
            LastName = "testuser1"
        };

        // Act
        var result = await createUserCommandHandler.Handle(createUserCommand, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Message.Should().Be("Could not create user");
    }

    [Fact]
    public async Task CreateUser_ShouldPublishUserChangedEvent_WhenUserIsCreatedSuccessfully()
    {
        // Arrange
        var logger = new Mock<ILogger<CreateUserCommandHandler>>();
        var publisher = new Mock<IPublisher>();
        Mock<IUserService> mock = new Mock<IUserService>();
        var userRequest = new UserRequest("", "testuser1", "testuser1@local", "testuser1", "testuser1", null!);
        var user = new UserVMWithLink("1", "testuser1", "testuser1@local", "testuser1", "testuser1", "/link?id=1");
        mock.Setup(x => x.CreateUser(It.IsAny<UserRequest>())).ReturnsAsync(Result.Ok(user));
        CreateUserCommandHandler createUserCommandHandler = new CreateUserCommandHandler(mock.Object, logger.Object, publisher.Object);
        var createUserCommand = new CreateUserCommand
        {
            UserName = "testuser1",
            Email = "testuser1@local",
            FirstName = "testuser1",
            LastName = "testuser1"
        };

        // Act
        var result = await createUserCommandHandler.Handle(createUserCommand, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        publisher.Verify(x => x.Publish(It.IsAny<UserChangedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteUser_Should_Return_Success()
    {
        // Arrange
        Mock<IUserService> mock = new Mock<IUserService>();
        mock.Setup(x => x.DeleteUser(It.IsAny<string>())).ReturnsAsync(Result.Ok());
        DeleteUserCommandHandler deleteUserCommandHandler = new DeleteUserCommandHandler(mock.Object);
        var deleteUserCommand = new DeleteUserCommand("1");

        // Act
        var result = await deleteUserCommandHandler.Handle(deleteUserCommand, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteUser_Should_Return_Failiure()
    {         // Arrange
        Mock<IUserService> mock = new Mock<IUserService>();
        mock.Setup(x => x.DeleteUser(It.IsAny<string>())).ReturnsAsync(Result.Fail("User not found"));
        DeleteUserCommandHandler deleteUserCommandHandler = new DeleteUserCommandHandler(mock.Object);
        var deleteUserCommand = new DeleteUserCommand("1");

        // Act
        var result = await deleteUserCommandHandler.Handle(deleteUserCommand, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Message.Should().Be("User not found");
    }

    [Fact]
    public async Task ResetPassword_Should_Return_Success()
    {
        // Arrange
        Mock<IUserService> mock = new Mock<IUserService>();
        mock.Setup(x => x.ResetPassword(It.IsAny<string>())).ReturnsAsync(Result.Ok());
        ResetPasswordCommandHandler resetPasswordUserCommandHandler = new ResetPasswordCommandHandler(mock.Object);
        var resetPasswordCommand = new ResetPasswordCommand("1");

        // Act
        var result = await resetPasswordUserCommandHandler.Handle(resetPasswordCommand, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task ResetPassword_Should_Return_Failiure()
    {
        // Arrange
        Mock<IUserService> mock = new Mock<IUserService>();
        mock.Setup(x => x.ResetPassword(It.IsAny<string>())).ReturnsAsync(Result.Fail("User not found"));
        ResetPasswordCommandHandler resetPasswordUserCommandHandler = new ResetPasswordCommandHandler(mock.Object);
        var resetPasswordCommand = new ResetPasswordCommand("1");

        // Act
        var result = await resetPasswordUserCommandHandler.Handle(resetPasswordCommand, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Message.Should().Be("User not found");
    }

    [Fact]
    public async Task ChangePassword_Should_Return_Success()
    {
        // Arrange
        Mock<IUserService> mock = new Mock<IUserService>();
        var changePasswordRequest = new ChangePasswordRequest("oldpassword", "!Newpassword123", "!Newpassword123");
        mock.Setup(x => x.ChangePassword(It.IsAny<ChangePasswordRequest>())).ReturnsAsync(Result.Ok());
        ChangePasswordCommandHandler changePasswordCommandHandler = new ChangePasswordCommandHandler(mock.Object);
        var changePasswordCommand = new ChangePasswordCommand
        {
            OldPassword = "oldpassword",
            NewPassword = "!Newpassword123",
            ConfirmPassword = "!Newpassword123"
        };

        // Act
        var result = await changePasswordCommandHandler.Handle(changePasswordCommand, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task ChangePassword_Should_Return_Failiure()
    {
        // Arrange
        Mock<IUserService> mock = new Mock<IUserService>();
        var changePasswordRequest = new ChangePasswordRequest("oldpassword", "!Newpassword123", "!Newpassword123");
        mock.Setup(x => x.ChangePassword(It.IsAny<ChangePasswordRequest>())).ReturnsAsync(Result.Fail("User not found"));
        ChangePasswordCommandHandler changePasswordCommandHandler = new ChangePasswordCommandHandler(mock.Object);
        var changePasswordCommand = new ChangePasswordCommand
        {
            OldPassword = "oldpassword",
            NewPassword = "!Newpassword123",
            ConfirmPassword = "!Newpassword123"

        };

        // Act
        var result = await changePasswordCommandHandler.Handle(changePasswordCommand, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Message.Should().Be("User not found");
    }

    [Fact]
    public async Task GetAccessControlConfiguration_Should_Return_Success()
    {
        // Arrange
        Mock<IUserService> mock = new Mock<IUserService>();
        var accessControlVm = new AccessControlVm(new List<RoleDto>
        {
            new RoleDto("1", "Admin", Permissions.All),
            new RoleDto("2", "User", Permissions.None)
        });
        mock.Setup(x => x.GetAccessControlConfiguration()).ReturnsAsync(Result.Ok(accessControlVm));
        GetAccessControlConfigurationQueryHandler getAccessControlConfigurationQueryHandler = new GetAccessControlConfigurationQueryHandler(mock.Object);
        var getAccessControlConfigurationQuery = new GetAccessControlConfigurationQuery();

        // Act
        var result = await getAccessControlConfigurationQueryHandler.Handle(getAccessControlConfigurationQuery, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Roles.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateAccessControlConfiguration_Should_Return_Success()
    {
        // Arrange
        Mock<IUserService> mock = new Mock<IUserService>();
        var roleRequest = new RoleRequest("1", "Admin", Permissions.All);
        mock.Setup(x => x.UpdateAccessControlConfiguration(It.IsAny<RoleRequest>())).ReturnsAsync(Result.Ok());
        UpdateAccessControlConfigurationCommandHandler updateAccessControlConfigurationCommandHandler = new UpdateAccessControlConfigurationCommandHandler(mock.Object);
        var updateAccessControlConfigurationCommand = new UpdateAccessControlConfigurationCommand
        {
            Id = "1",
            Permission = Permissions.All
        };

        // Act
        var result = await updateAccessControlConfigurationCommandHandler.Handle(updateAccessControlConfigurationCommand, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task AccessControlConfiguration_Should_Return_Failiure()
    {
        // Arrange
        Mock<IUserService> mock = new Mock<IUserService>();
        mock.Setup(x => x.UpdateAccessControlConfiguration(It.IsAny<RoleRequest>())).ReturnsAsync(Result.Fail("Role not found"));
        UpdateAccessControlConfigurationCommandHandler updateAccessControlConfigurationCommandHandler = new UpdateAccessControlConfigurationCommandHandler(mock.Object);
        var updateAccessControlConfigurationCommand = new UpdateAccessControlConfigurationCommand
        {
            Id = "1",
            Permission = Permissions.All
        };

        // Act
        var result = await updateAccessControlConfigurationCommandHandler.Handle(updateAccessControlConfigurationCommand, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Message.Should().Be("Role not found");
    }
    [Fact]
    public async Task GetRoles_Should_Return_Success()
    {
        // Arrange
        Mock<IUserService> mock = new Mock<IUserService>();
        var roles = new List<RoleDto>
        {
            new RoleDto("1", "Admin", Permissions.All),
            new RoleDto("2", "User", Permissions.None)
        };
        mock.Setup(x => x.GetRoles()).ReturnsAsync(Result.Ok<IEnumerable<RoleDto>>(roles));
        GetRolesQueryHandler getRolesQueryHandler = new GetRolesQueryHandler(mock.Object);

        // Act
        var result = await getRolesQueryHandler.Handle(new GetRolesQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task CreateRole_Should_Return_Success()
    {
        // Arrange
        Mock<IUserService> mock = new Mock<IUserService>();
        var role = new RoleDto("1", "Admin", Permissions.All);
        mock.Setup(x => x.CreateRole(It.IsAny<RoleRequest>())).ReturnsAsync(Result.Ok(role));
        CreateRoleCommandHandler createRoleCommandHandler = new CreateRoleCommandHandler(mock.Object);
        var createRoleCommand = new CreateRoleCommand
        {
            Name = "Admin",
            Permissions = Permissions.All
        };

        // Act
        var result = await createRoleCommandHandler.Handle(createRoleCommand, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be("1");
    }

    [Fact]
    public async Task CreateRole_Should_Return_Failiure()
    {
        // Arrange
        Mock<IUserService> mock = new Mock<IUserService>();
        mock.Setup(x => x.CreateRole(It.IsAny<RoleRequest>())).ReturnsAsync(Result.Fail<RoleDto>("Role not found"));
        CreateRoleCommandHandler createRoleCommandHandler = new CreateRoleCommandHandler(mock.Object);
        var createRoleCommand = new CreateRoleCommand
        {
            Name = "Admin",
            Permissions = Permissions.All
        };

        // Act
        var result = await createRoleCommandHandler.Handle(createRoleCommand, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Message.Should().Be("Role not found");
    }

    [Fact]
    public async Task UpdateRole_Should_Return_Success()
    {
        // Arrange
        Mock<IUserService> mock = new Mock<IUserService>();
        mock.Setup(x => x.UpdateRole(It.IsAny<RoleRequest>())).ReturnsAsync(Result.Ok());
        UpdateRoleCommandHandler updateRoleCommandHandler = new UpdateRoleCommandHandler(mock.Object);
        var updateRoleCommand = new UpdateRoleCommand
        {
            Id = "1",
            Name = "Admin",
            Permissions = Permissions.All
        };

        // Act
        var result = await updateRoleCommandHandler.Handle(updateRoleCommand, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateRole_Should_Return_Failiure()
    {
        // Arrange
        Mock<IUserService> mock = new Mock<IUserService>();
        mock.Setup(x => x.UpdateRole(It.IsAny<RoleRequest>())).ReturnsAsync(Result.Fail("Role not found"));
        UpdateRoleCommandHandler updateRoleCommandHandler = new UpdateRoleCommandHandler(mock.Object);
        var updateRoleCommand = new UpdateRoleCommand
        {
            Id = "1",
            Name = "Admin",
            Permissions = Permissions.All
        };

        // Act
        var result = await updateRoleCommandHandler.Handle(updateRoleCommand, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Message.Should().Be("Role not found");
    }

    [Fact]
    public async Task DeleteRole_Should_Return_Success()
    {
        // Arrange
        Mock<IUserService> mock = new Mock<IUserService>();
        mock.Setup(x => x.DeleteRole(It.IsAny<string>())).ReturnsAsync(Result.Ok());
        DeleteRoleCommandHandler deleteRoleCommandHandler = new DeleteRoleCommandHandler(mock.Object);
        var deleteRoleCommand = new DeleteRoleCommand("1");

        // Act
        var result = await deleteRoleCommandHandler.Handle(deleteRoleCommand, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteRole_Should_Return_Failiure()
    {
        // Arrange
        Mock<IUserService> mock = new Mock<IUserService>();
        mock.Setup(x => x.DeleteRole(It.IsAny<string>())).ReturnsAsync(Result.Fail("Role not found"));
        DeleteRoleCommandHandler deleteRoleCommandHandler = new DeleteRoleCommandHandler(mock.Object);
        var deleteRoleCommand = new DeleteRoleCommand("1");

        // Act
        var result = await deleteRoleCommandHandler.Handle(deleteRoleCommand, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Message.Should().Be("Role not found");
    }

    [Fact]
    public async Task ConfirmEmal_Should_Return_Success()
    {
        // Arrange
        Mock<IUserService> mock = new Mock<IUserService>();
        mock.Setup(x => x.ConfirmEmail(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(Result.Ok());
        ConfirmEmailCommandHandler confirmEmailCommandHandler = new ConfirmEmailCommandHandler(mock.Object);
        var confirmEmailCommand = new ConfirmEmailCommand("1", "token");

        // Act
        var result = await confirmEmailCommandHandler.Handle(confirmEmailCommand, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }


    [Fact]
    public async Task ForgotPassword_Should_Return_Success()
    {
        // Arrange
        var user = new UserVMWithLink("1", "testuser1", "testuser1@local", "testuser1", "testuser1", "/link?id=1");
        var publisher = new Mock<IPublisher>();
        Mock<IUserService> mock = new Mock<IUserService>();
        mock.Setup(x => x.ForgotPassword(It.IsAny<string>())).ReturnsAsync(Result.Ok(user));
        ForgotPasswordCommandHandler forgotPasswordCommandHandler = new ForgotPasswordCommandHandler(mock.Object, publisher.Object);
        ForgotPasswordCommand forgotPasswordCommand = new ForgotPasswordCommand("user@test.com");

        // Act
        var result = await forgotPasswordCommandHandler.Handle(forgotPasswordCommand, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task ForgotPassword_Should_Return_NotFoundFailiur()
    {
        // Arrange
        var publisher = new Mock<IPublisher>();
        Mock<IUserService> mock = new Mock<IUserService>();
        mock.Setup(x => x.ForgotPassword(It.IsAny<string>())).ReturnsAsync(Result.Fail("User not found"));
        ForgotPasswordCommandHandler forgotPasswordCommandHandler = new ForgotPasswordCommandHandler(mock.Object, publisher.Object);
        ForgotPasswordCommand forgotPasswordCommand = new ForgotPasswordCommand("user@test.com");

        // Act
        var result = await forgotPasswordCommandHandler.Handle(forgotPasswordCommand, CancellationToken.None);

        // Assert
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Message.Should().Be("User not found");
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task ForgotPassword_ShouldPublishUserChangedEvent_WhenForgotPasswordSuccessfully()
    {
        // Arrange
        var user = new UserVMWithLink("1", "testuser1", "testuser1@local", "testuser1", "testuser1", "/link?id=1");
        var publisher = new Mock<IPublisher>();
        Mock<IUserService> mock = new Mock<IUserService>();
        mock.Setup(x => x.ForgotPassword(It.IsAny<string>())).ReturnsAsync(Result.Ok(user));
        ForgotPasswordCommandHandler forgotPasswordCommandHandler = new ForgotPasswordCommandHandler(mock.Object, publisher.Object);
        ForgotPasswordCommand forgotPasswordCommand = new ForgotPasswordCommand("user@test.com");

        // Act
        var result = await forgotPasswordCommandHandler.Handle(forgotPasswordCommand, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        publisher.Verify(x => x.Publish(It.IsAny<UserChangedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ResetUserPassword_Should_Return_Success()
    {
        // Arrange
        Mock<IUserService> mock = new Mock<IUserService>();
        mock.Setup(x => x.ResetUserPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(Result.Ok());
        ResetUserPasswordCommandHandler resetPasswordCommandHandler = new ResetUserPasswordCommandHandler(mock.Object);
        ResetUserPasswordCommand resetPasswordCommand = new ResetUserPasswordCommand("1", "token", "!Password1");

        // Act
        var result = await resetPasswordCommandHandler.Handle(resetPasswordCommand, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task ResetPassword_Should_Return_NotFoundFailiur()
    {
        // Arrange
        Mock<IUserService> mock = new Mock<IUserService>();
        mock.Setup(x => x.ResetUserPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(Result.Fail("User not found"));
        ResetUserPasswordCommandHandler resetPasswordCommandHandler = new ResetUserPasswordCommandHandler(mock.Object);
        ResetUserPasswordCommand resetPasswordCommand = new ResetUserPasswordCommand("1", "token", "!Password1");

        // Act
        var result = await resetPasswordCommandHandler.Handle(resetPasswordCommand, CancellationToken.None);

        // Assert
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Message.Should().Be("User not found");
        result.IsFailed.Should().BeTrue();
    }
}
