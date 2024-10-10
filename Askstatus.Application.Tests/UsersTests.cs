﻿using Askstatus.Application.Interfaces;
using Askstatus.Application.Users;
using Askstatus.Common.Users;
using FluentAssertions;
using FluentResults;
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
        var userRequest = new UserRequest("1", "testuser1", "testuser1@local", "testuser1", "testuser1");
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
        var userRequest = new UserRequest("1", "testuser1", "testuser1@local", "testuser1", "testuser1");
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
        Mock<IUserService> mock = new Mock<IUserService>();
        var userRequest = new UserRequest("", "testuser1", "testuser1@local", "testuser1", "testuser1");
        var user = new UserVM("1", "testuser1", "testuser1@local", "testuser1", "testuser1");
        mock.Setup(x => x.CreateUser(It.IsAny<UserRequest>())).ReturnsAsync(Result.Ok(user));
        CreateUserCommandHandler createUserCommandHandler = new CreateUserCommandHandler(mock.Object);
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
        Mock<IUserService> mock = new Mock<IUserService>();
        var userRequest = new UserRequest("", "testuser1", "testuser1@local", "testuser1", "testuser1");
        mock.Setup(x => x.CreateUser(It.IsAny<UserRequest>())).ReturnsAsync(Result.Fail("Could not create user"));
        CreateUserCommandHandler createUserCommandHandler = new CreateUserCommandHandler(mock.Object);
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
        var changePasswordRequest = new ChangePasswordRequest("oldpassword", "newpassword");
        mock.Setup(x => x.ChangePassword(It.IsAny<ChangePasswordRequest>())).ReturnsAsync(Result.Ok());
        ChangePasswordCommandHandler changePasswordCommandHandler = new ChangePasswordCommandHandler(mock.Object);
        var changePasswordCommand = new ChangePasswordCommand
        {
            OldPassword = "oldpassword",
            NewPassword = "newpassword"
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
        var changePasswordRequest = new ChangePasswordRequest("oldpassword", "newpassword");
        mock.Setup(x => x.ChangePassword(It.IsAny<ChangePasswordRequest>())).ReturnsAsync(Result.Fail("User not found"));
        ChangePasswordCommandHandler changePasswordCommandHandler = new ChangePasswordCommandHandler(mock.Object);
        var changePasswordCommand = new ChangePasswordCommand
        {
            OldPassword = "oldpassword",
            NewPassword = "newpassword"
        };

        // Act
        var result = await changePasswordCommandHandler.Handle(changePasswordCommand, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Message.Should().Be("User not found");
    }

}