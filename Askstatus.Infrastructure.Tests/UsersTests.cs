using System.Security.Claims;
using Askstatus.Common.Authorization;
using Askstatus.Common.Users;
using Askstatus.Domain.Constants;
using Askstatus.Infrastructure.Identity;
using Askstatus.Infrastructure.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MockQueryable;
using Moq;

namespace Askstatus.Infrastructure.Tests;
public class UsersTests
{
    [Fact]
    public async Task GetUsers_Should_Return_Success()
    {
        // Arrange
        Mock<UserManager<ApplicationUser>> userManagerMock = MockUserManager();
        Mock<SignInManager<ApplicationUser>> signInManagerMock = MockSignInManager(userManagerMock.Object);
        Mock<RoleManager<ApplicationRole>> roleManagerMock = MockRoleManager();
        var usersService = new UserService(signInManagerMock.Object, roleManagerMock.Object, new Mock<ILogger<UserService>>().Object);

        // Act
        var result = await usersService.GetUsers();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(2);
        result.Value.First().UserName.Should().Be("admin");
    }

    [Fact]
    public async Task GetUserById_Should_Return_Success()
    {
        // Arrange
        Mock<UserManager<ApplicationUser>> userManagerMock = MockUserManager();
        Mock<SignInManager<ApplicationUser>> signInManagerMock = MockSignInManager(userManagerMock.Object);
        Mock<RoleManager<ApplicationRole>> roleManagerMock = MockRoleManager();
        var usersService = new UserService(signInManagerMock.Object, roleManagerMock.Object, new Mock<ILogger<UserService>>().Object);

        // Act
        var result = await usersService.GetUserById("1");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.UserName.Should().Be("admin");
    }

    [Fact]
    public async Task GetUserById_Should_Return_Failiure()
    {
        // Arrange
        Mock<UserManager<ApplicationUser>> userManagerMock = MockUserManager();
        Mock<SignInManager<ApplicationUser>> signInManagerMock = MockSignInManager(userManagerMock.Object);
        Mock<RoleManager<ApplicationRole>> roleManagerMock = MockRoleManager();
        var usersService = new UserService(signInManagerMock.Object, roleManagerMock.Object, new Mock<ILogger<UserService>>().Object);

        // Act
        var result = await usersService.GetUserById("3");

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.First().Message.Should().Be("User not found");
    }

    [Fact]
    public async Task UpdateUser_Should_Return_Success()
    {
        // Arrange
        var roles = new List<string> { "Admin" };
        Mock<UserManager<ApplicationUser>> userManagerMock = MockUserManager();
        Mock<SignInManager<ApplicationUser>> signInManagerMock = MockSignInManager(userManagerMock.Object);
        Mock<RoleManager<ApplicationRole>> roleManagerMock = MockRoleManager();
        userManagerMock.Setup(userManager => userManager.
            FindByIdAsync(It.IsAny<string>())).
            ReturnsAsync(new ApplicationUser { Id = "1", UserName = "adminb", Email = "adminb@localhost.local", FirstName = "Adminb", LastName = "User" });
        userManagerMock.Setup(userManager => userManager
            .UpdateAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(IdentityResult.Success);
        userManagerMock.Setup(userManager => userManager
            .GetRolesAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(roles);
        userManagerMock.Setup(userManager => userManager
            .AddToRolesAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(IdentityResult.Success);
        userManagerMock.Setup(userManager => userManager
            .RemoveFromRolesAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(IdentityResult.Success);
        var usersService = new UserService(signInManagerMock.Object, roleManagerMock.Object, new Mock<ILogger<UserService>>().Object);

        // Act
        var result = await usersService.UpdateUser(new UserRequest("1", "adminb", "adminb@localhost.local", "Adminb", "User", roles));

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateUser_Should_ReturnNotFound_Failiure()
    {
        // Arrange
        Mock<UserManager<ApplicationUser>> userManagerMock = MockUserManager();
        Mock<SignInManager<ApplicationUser>> signInManagerMock = MockSignInManager(userManagerMock.Object);
        Mock<RoleManager<ApplicationRole>> roleManagerMock = MockRoleManager();
        userManagerMock.Setup(userManager => userManager
            .FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(() => null);
        var usersService = new UserService(signInManagerMock.Object, roleManagerMock.Object, new Mock<ILogger<UserService>>().Object);
        var roles = new List<string> { "Admin" };

        // Act
        var result = await usersService.UpdateUser(new UserRequest("3", "adminb", "adminb@localhost.local", "Adminb", "User", roles));

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.First().Message.Should().Be("User not found");
    }

    [Fact]
    public async Task UpdateUser_Should_Return_Failiure_When_AddRoles_Fails()
    {
        // Arrange
        var roles = new List<string> { "Admin" };
        var loggerMock = new Mock<ILogger<UserService>>();
        Mock<UserManager<ApplicationUser>> userManagerMock = MockUserManager();
        Mock<SignInManager<ApplicationUser>> signInManagerMock = MockSignInManager(userManagerMock.Object);
        Mock<RoleManager<ApplicationRole>> roleManagerMock = MockRoleManager();
        userManagerMock.Setup(userManager => userManager.
            FindByIdAsync(It.IsAny<string>())).
            ReturnsAsync(new ApplicationUser { Id = "1", UserName = "adminb", Email = "adminb@localhost.local", FirstName = "Adminb", LastName = "User" });
        userManagerMock.Setup(userManager => userManager
            .UpdateAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(IdentityResult.Success);
        userManagerMock.Setup(userManager => userManager
            .GetRolesAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(new List<string>());
        userManagerMock.Setup(userManager => userManager
            .AddToRolesAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "Error", Description = "Error" }));
        var usersService = new UserService(signInManagerMock.Object, roleManagerMock.Object, loggerMock.Object);

        // Act
        var result = await usersService.UpdateUser(new UserRequest("3", "adminb", "adminb@localhost.local", "Adminb", "User", roles));

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.First().Message.Should().Be("Could not update user");
        loggerMock.Verify(l =>
        l.Log(LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Could not update user adminb")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
        loggerMock.Verify(l =>
        l.Log(LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Error: Error | Code: Error")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
    }

    [Fact]
    public async Task UpdateUser_Should_Return_Failiure_When_RemoveRoles_Fails()
    {
        // Arrange
        var roles = new List<string> { "Admin" };
        var loggerMock = new Mock<ILogger<UserService>>();
        Mock<UserManager<ApplicationUser>> userManagerMock = MockUserManager();
        Mock<SignInManager<ApplicationUser>> signInManagerMock = MockSignInManager(userManagerMock.Object);
        Mock<RoleManager<ApplicationRole>> roleManagerMock = MockRoleManager();
        userManagerMock.Setup(userManager => userManager.
            FindByIdAsync(It.IsAny<string>())).
            ReturnsAsync(new ApplicationUser { Id = "1", UserName = "adminb", Email = "adminb@localhost.local", FirstName = "Adminb", LastName = "User" });
        userManagerMock.Setup(userManager => userManager
            .UpdateAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(IdentityResult.Success);
        userManagerMock.Setup(userManager => userManager
            .GetRolesAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(new List<string>() { "User" });
        userManagerMock.Setup(userManager => userManager
            .AddToRolesAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(IdentityResult.Success);
        userManagerMock.Setup(userManager => userManager
            .RemoveFromRolesAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "Error", Description = "Error" }));
        var usersService = new UserService(signInManagerMock.Object, roleManagerMock.Object, loggerMock.Object);

        // Act
        var result = await usersService.UpdateUser(new UserRequest("3", "adminb", "adminb@localhost.local", "Adminb", "User", roles));

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.First().Message.Should().Be("Could not update user");
        loggerMock.Verify(l =>
        l.Log(LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Could not update user adminb")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
        loggerMock.Verify(l =>
        l.Log(LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Error: Error | Code: Error")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
    }

    [Fact]
    public async Task UpdateUser_Should_Return_Failiure()
    {
        // Arrange
        var roles = new List<string> { "Admin" };
        var loggerMock = new Mock<ILogger<UserService>>();
        Mock<UserManager<ApplicationUser>> userManagerMock = MockUserManager();
        Mock<SignInManager<ApplicationUser>> signInManagerMock = MockSignInManager(userManagerMock.Object);
        Mock<RoleManager<ApplicationRole>> roleManagerMock = MockRoleManager();
        userManagerMock.Setup(userManager => userManager.
            FindByIdAsync(It.IsAny<string>())).
            ReturnsAsync(new ApplicationUser { Id = "1", UserName = "adminb", Email = "adminb@localhost.local", FirstName = "Adminb", LastName = "User" });
        userManagerMock.Setup(userManager => userManager
            .UpdateAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "Error", Description = "Error" }));
        var usersService = new UserService(signInManagerMock.Object, roleManagerMock.Object, loggerMock.Object);

        // Act
        var result = await usersService.UpdateUser(new UserRequest("3", "adminb", "adminb@localhost.local", "Adminb", "User", roles));

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.First().Message.Should().Be("Could not update user");
        loggerMock.Verify(l =>
        l.Log(LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Could not update user adminb")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
        loggerMock.Verify(l =>
        l.Log(LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Error: Error | Code: Error")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
    }

    [Fact]
    public async Task CreateUser_Should_Return_Success()
    {
        // Arrange
        Mock<UserManager<ApplicationUser>> userManagerMock = MockUserManager();
        Mock<SignInManager<ApplicationUser>> signInManagerMock = MockSignInManager(userManagerMock.Object);
        Mock<RoleManager<ApplicationRole>> roleManagerMock = MockRoleManager();
        userManagerMock.Setup(userManager => userManager.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
        var usersService = new UserService(signInManagerMock.Object, roleManagerMock.Object, new Mock<ILogger<UserService>>().Object);

        // Act
        var result = await usersService.CreateUser(new UserRequest(string.Empty, "adminb", "adminb@localhost.local", "Adminb", "User", null!));

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task CreateUser_Should_Return_Failiure()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<UserService>>();
        Mock<UserManager<ApplicationUser>> userManagerMock = MockUserManager();
        Mock<SignInManager<ApplicationUser>> signInManagerMock = MockSignInManager(userManagerMock.Object);
        Mock<RoleManager<ApplicationRole>> roleManagerMock = MockRoleManager();
        userManagerMock.Setup(userManager => userManager
            .CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "Error", Description = "Error" }));
        var usersService = new UserService(signInManagerMock.Object, roleManagerMock.Object, loggerMock.Object);

        // Act
        var result = await usersService.CreateUser(new UserRequest(string.Empty, "adminb", "adminb@localhost.local", "Adminb", "User", null!));

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.First().Message.Should().Be("Culd not create user");
        //loggerMock.Verify(logger => logger.LogWarning("Could not create user {User}", "adminb"), Times.Once);
        loggerMock.Verify(l =>
        l.Log(LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Could not create user adminb")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
        loggerMock.Verify(l =>
        l.Log(LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Error: Error | Code: Error")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
    }

    [Fact]
    public async Task DeleteUser_Should_Return_Success()
    {
        // Arrange
        Mock<UserManager<ApplicationUser>> userManagerMock = MockUserManager();
        Mock<SignInManager<ApplicationUser>> signInManagerMock = MockSignInManager(userManagerMock.Object);
        Mock<RoleManager<ApplicationRole>> roleManagerMock = MockRoleManager();
        userManagerMock.Setup(userManager => userManager
            .FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new ApplicationUser { Id = "1", UserName = "adminb", Email = "adminb@localhost.local", FirstName = "Adminb", LastName = "User" });
        userManagerMock.Setup(userManager => userManager.DeleteAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);
        var usersService = new UserService(signInManagerMock.Object, roleManagerMock.Object, new Mock<ILogger<UserService>>().Object);

        // Act
        var result = await usersService.DeleteUser("1");

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteUser_Should_Return_NotFound_Failiure()
    {
        // Arrange
        Mock<UserManager<ApplicationUser>> userManagerMock = MockUserManager();
        Mock<SignInManager<ApplicationUser>> signInManagerMock = MockSignInManager(userManagerMock.Object);
        Mock<RoleManager<ApplicationRole>> roleManagerMock = MockRoleManager();
        userManagerMock.Setup(userManager => userManager.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(() => null);
        var usersService = new UserService(signInManagerMock.Object, roleManagerMock.Object, new Mock<ILogger<UserService>>().Object);

        // Act
        var result = await usersService.DeleteUser("3");

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.First().Message.Should().Be("User not found");
    }

    [Fact]
    public async Task DeleteUser_Should_Return_CantDeleteAdmin_Failiure()
    {
        // Arrange
        Mock<UserManager<ApplicationUser>> userManagerMock = MockUserManager();
        Mock<SignInManager<ApplicationUser>> signInManagerMock = MockSignInManager(userManagerMock.Object);
        Mock<RoleManager<ApplicationRole>> roleManagerMock = MockRoleManager();
        userManagerMock.Setup(userManager => userManager
            .FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new ApplicationUser { Id = "1", UserName = "admin", Email = "admin@localhost.local", FirstName = "Admin", LastName = "User" });
        var usersService = new UserService(signInManagerMock.Object, roleManagerMock.Object, new Mock<ILogger<UserService>>().Object);

        // Act
        var result = await usersService.DeleteUser("1");

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.First().Message.Should().Be("Cannot delete default admin user");
    }

    [Fact]
    public async Task DeleteUser_Should_Return_Failiure()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<UserService>>();
        Mock<UserManager<ApplicationUser>> userManagerMock = MockUserManager();
        Mock<SignInManager<ApplicationUser>> signInManagerMock = MockSignInManager(userManagerMock.Object);
        Mock<RoleManager<ApplicationRole>> roleManagerMock = MockRoleManager();
        userManagerMock.Setup(userManager => userManager
            .FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new ApplicationUser { Id = "1", UserName = "adminb", Email = "adminb@localhost.local", FirstName = "Adminb", LastName = "User" });
        userManagerMock.Setup(userManager => userManager
            .DeleteAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "Error", Description = "Error" }));
        var usersService = new UserService(signInManagerMock.Object, roleManagerMock.Object, loggerMock.Object);

        // Act
        var result = await usersService.DeleteUser("1");

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.First().Message.Should().Be("Could not delete user");
        loggerMock.Verify(l =>
        l.Log(LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Could not delete user adminb")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
        loggerMock.Verify(l =>
        l.Log(LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Error: Error | Code: Error")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
    }

    [Fact]
    public async Task ResetPassword_Should_Return_Success()
    {
        // Arrange
        Mock<UserManager<ApplicationUser>> userManagerMock = MockUserManager();
        Mock<SignInManager<ApplicationUser>> signInManagerMock = MockSignInManager(userManagerMock.Object);
        Mock<RoleManager<ApplicationRole>> roleManagerMock = MockRoleManager();
        userManagerMock.Setup(userManager => userManager
            .FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new ApplicationUser { Id = "1", UserName = "adminb", Email = "adminb@localhost.local", FirstName = "Adminb", LastName = "User" });
        userManagerMock.Setup(userManager => userManager
            .GeneratePasswordResetTokenAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync("token");
        userManagerMock.Setup(userManager => userManager.
            ResetPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        var usersService = new UserService(signInManagerMock.Object, roleManagerMock.Object, new Mock<ILogger<UserService>>().Object);

        // Act
        var result = await usersService.ResetPassword("1");

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task ResetPassword_Should_Return_NotFound_Failiure()
    {
        // Arrange
        Mock<UserManager<ApplicationUser>> userManagerMock = MockUserManager();
        Mock<SignInManager<ApplicationUser>> signInManagerMock = MockSignInManager(userManagerMock.Object);
        Mock<RoleManager<ApplicationRole>> roleManagerMock = MockRoleManager();
        userManagerMock.Setup(userManager => userManager.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(() => null);
        var usersService = new UserService(signInManagerMock.Object, roleManagerMock.Object, new Mock<ILogger<UserService>>().Object);

        // Act
        var result = await usersService.ResetPassword("1");

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.First().Message.Should().Be("User not found");
    }

    [Fact]
    public async Task ResetPassword_Should_Return_Failiure()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<UserService>>();
        Mock<UserManager<ApplicationUser>> userManagerMock = MockUserManager();
        Mock<SignInManager<ApplicationUser>> signInManagerMock = MockSignInManager(userManagerMock.Object);
        Mock<RoleManager<ApplicationRole>> roleManagerMock = MockRoleManager();
        userManagerMock.Setup(userManager => userManager
            .FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new ApplicationUser { Id = "1", UserName = "adminb", Email = "adminb@localhost.local", FirstName = "Adminb", LastName = "User" });
        userManagerMock.Setup(userManager => userManager
            .GeneratePasswordResetTokenAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync("token");
        userManagerMock.Setup(userManager => userManager.
            ResetPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "Error", Description = "Error" }));
        var usersService = new UserService(signInManagerMock.Object, roleManagerMock.Object, loggerMock.Object);

        // Act
        var result = await usersService.ResetPassword("1");

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.First().Message.Should().Be("Could not reset password");
        loggerMock.Verify(l =>
        l.Log(LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Could not reset password for user adminb")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
        loggerMock.Verify(l =>
        l.Log(LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Error: Error | Code: Error")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
    }

    [Fact]
    public async Task ChangePassword_Should_Return_Success()
    {
        // Arrange
        Mock<UserManager<ApplicationUser>> userManagerMock = MockUserManager();
        Mock<SignInManager<ApplicationUser>> signInManagerMock = MockSignInManager(userManagerMock.Object);
        Mock<RoleManager<ApplicationRole>> roleManagerMock = MockRoleManager();
        userManagerMock.Setup(userManager => userManager
            .FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new ApplicationUser { Id = "1", UserName = "adminb", Email = "adminb@localhost.local", FirstName = "Adminb", LastName = "User" });
        userManagerMock.Setup(userManager => userManager
            .ChangePasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        var usersService = new UserService(signInManagerMock.Object, roleManagerMock.Object, new Mock<ILogger<UserService>>().Object);

        // Act
        var result = await usersService.ChangePassword(new ChangePasswordRequest("OldPassword", "NewPassword"));
    }

    [Fact]
    public async Task ChangePassword_Should_Return_NotFound_Failiure()
    {
        //Arrange
        Mock<UserManager<ApplicationUser>> userManagerMock = MockUserManager();
        Mock<SignInManager<ApplicationUser>> signInManagerMock = MockSignInManager(userManagerMock.Object);
        Mock<RoleManager<ApplicationRole>> roleManagerMock = MockRoleManager();
        userManagerMock.Setup(userManager => userManager.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(() => null);
        var usersService = new UserService(signInManagerMock.Object, roleManagerMock.Object, new Mock<ILogger<UserService>>().Object);

        // Act
        var result = await usersService.ChangePassword(new ChangePasswordRequest("OldPassword", "NewPassword"));

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.First().Message.Should().Be("User not found");
    }

    [Fact]
    public async Task ChangePassword_Should_Return_Failiure()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<UserService>>();
        Mock<UserManager<ApplicationUser>> userManagerMock = MockUserManager();
        Mock<SignInManager<ApplicationUser>> signInManagerMock = MockSignInManager(userManagerMock.Object);
        Mock<RoleManager<ApplicationRole>> roleManagerMock = MockRoleManager();
        userManagerMock.Setup(userManager => userManager
            .FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new ApplicationUser { Id = "1", UserName = "adminb", Email = "adminb@localhost.local", FirstName = "Adminb", LastName = "User" });
        userManagerMock.Setup(userManager => userManager
            .ChangePasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "Error", Description = "Error" }));
        var usersService = new UserService(signInManagerMock.Object, roleManagerMock.Object, loggerMock.Object);

        // Act
        var result = await usersService.ChangePassword(new ChangePasswordRequest("OldPassword", "NewPassword"));

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.First().Message.Should().Be("Could not change password");
        loggerMock.Verify(l =>
        l.Log(LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Could not change password for user adminb")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
        loggerMock.Verify(l =>
        l.Log(LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Error: Error | Code: Error")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
    }

    [Fact]
    public async Task GetAccessControlConfiguration_Should_Return_Success()
    {
        // Arrange
        Mock<UserManager<ApplicationUser>> userManagerMock = MockUserManager();
        Mock<SignInManager<ApplicationUser>> signInManagerMock = MockSignInManager(userManagerMock.Object);
        Mock<RoleManager<ApplicationRole>> roleManagerMock = MockRoleManager();
        var usersService = new UserService(signInManagerMock.Object, roleManagerMock.Object, new Mock<ILogger<UserService>>().Object);

        // Act
        var result = await usersService.GetAccessControlConfiguration();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Roles.Should().NotBeEmpty();
        result.Value.Roles.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateAccessControlConfiguration_Should_Return_Success()
    {
        // Arrange
        Mock<UserManager<ApplicationUser>> userManagerMock = MockUserManager();
        Mock<SignInManager<ApplicationUser>> signInManagerMock = MockSignInManager(userManagerMock.Object);
        Mock<RoleManager<ApplicationRole>> roleManagerMock = MockRoleManager();
        var usersService = new UserService(signInManagerMock.Object, roleManagerMock.Object, new Mock<ILogger<UserService>>().Object);
        roleManagerMock.Setup(roleManager => roleManager.
            FindByIdAsync(It.IsAny<string>())).
            ReturnsAsync(new ApplicationRole { Id = "1", Name = "Admin", Permissions = Permissions.All });
        roleManagerMock.Setup(roleManager => roleManager.UpdateAsync(It.IsAny<ApplicationRole>())).ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await usersService.UpdateAccessControlConfiguration(new RoleRequest("1", "Admin", Permissions.All));

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAccessControlConfiguration_Should_Return_Failiure()
    {
        // Arrange
        Mock<UserManager<ApplicationUser>> userManagerMock = MockUserManager();
        Mock<SignInManager<ApplicationUser>> signInManagerMock = MockSignInManager(userManagerMock.Object);
        Mock<RoleManager<ApplicationRole>> roleManagerMock = MockRoleManager();
        var usersService = new UserService(signInManagerMock.Object, roleManagerMock.Object, new Mock<ILogger<UserService>>().Object);
        roleManagerMock.Setup(roleManager => roleManager
            .FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(() => null);

        // Act
        var result = await usersService.UpdateAccessControlConfiguration(new RoleRequest("1", "Admin", Permissions.All));

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.First().Message.Should().Be("Role not found");
    }

    [Fact]
    public async Task UpdateAccessControlConfiguration_Should_Return_NotFound_Failiure()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<UserService>>();
        Mock<UserManager<ApplicationUser>> userManagerMock = MockUserManager();
        Mock<SignInManager<ApplicationUser>> signInManagerMock = MockSignInManager(userManagerMock.Object);
        Mock<RoleManager<ApplicationRole>> roleManagerMock = MockRoleManager();
        var usersService = new UserService(signInManagerMock.Object, roleManagerMock.Object, loggerMock.Object);
        roleManagerMock.Setup(roleManager => roleManager
            .FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new ApplicationRole { Id = "1", Name = "Admin", Permissions = Permissions.All });
        roleManagerMock.Setup(roleManager => roleManager.UpdateAsync(It.IsAny<ApplicationRole>())).ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "Error", Description = "Error" }));

        // Act
        var result = await usersService.UpdateAccessControlConfiguration(new RoleRequest("1", "Admin", Permissions.All));

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.First().Message.Should().Be("Could not update role");
        loggerMock.Verify(l =>
        l.Log(LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Could not update role Admin")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
        loggerMock.Verify(l =>
        l.Log(LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Error: Error | Code: Error")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
    }

    [Fact]
    public async Task GetRoles_Should_Return_Success()
    {
        // Arrange
        Mock<UserManager<ApplicationUser>> userManagerMock = MockUserManager();
        Mock<SignInManager<ApplicationUser>> signInManagerMock = MockSignInManager(userManagerMock.Object);
        Mock<RoleManager<ApplicationRole>> roleManagerMock = MockRoleManager();
        var usersService = new UserService(signInManagerMock.Object, roleManagerMock.Object, new Mock<ILogger<UserService>>().Object);

        // Act
        var result = await usersService.GetRoles();

        // Assert
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(2);
        result.Value.First().Name.Should().Be("Admin");
    }

    [Fact]
    public async Task CreateRoles_Should_Return_Success()
    {
        // Arrange
        Mock<UserManager<ApplicationUser>> userManagerMock = MockUserManager();
        Mock<SignInManager<ApplicationUser>> signInManagerMock = MockSignInManager(userManagerMock.Object);
        Mock<RoleManager<ApplicationRole>> roleManagerMock = MockRoleManager();
        roleManagerMock.Setup(roleManager => roleManager
            .CreateAsync(It.IsAny<ApplicationRole>()))
            .ReturnsAsync(IdentityResult.Success);
        var usersService = new UserService(signInManagerMock.Object, roleManagerMock.Object, new Mock<ILogger<UserService>>().Object);

        // Act
        var result = await usersService.CreateRole(new RoleRequest(string.Empty, "Admin", Permissions.All));

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Name.Should().Be("Admin");
    }

    [Fact]
    public async Task CreateRoles_Should_Return_Failiure()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<UserService>>();
        Mock<UserManager<ApplicationUser>> userManagerMock = MockUserManager();
        Mock<SignInManager<ApplicationUser>> signInManagerMock = MockSignInManager(userManagerMock.Object);
        Mock<RoleManager<ApplicationRole>> roleManagerMock = MockRoleManager();
        roleManagerMock.Setup(roleManager => roleManager
            .CreateAsync(It.IsAny<ApplicationRole>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "Error", Description = "Error" }));
        var usersService = new UserService(signInManagerMock.Object, roleManagerMock.Object, loggerMock.Object);

        // Act
        var result = await usersService.CreateRole(new RoleRequest(string.Empty, "Admin", Permissions.All));

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.First().Message.Should().Be("Could not create role");
        loggerMock.Verify(l =>
        l.Log(LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Could not create role Admin")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
        loggerMock.Verify(l =>
        l.Log(LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Error: Error | Code: Error")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
    }

    [Fact]
    public async Task UpdateRoles_Should_Return_Success()
    {
        // Arrange
        Mock<UserManager<ApplicationUser>> userManagerMock = MockUserManager();
        Mock<SignInManager<ApplicationUser>> signInManagerMock = MockSignInManager(userManagerMock.Object);
        Mock<RoleManager<ApplicationRole>> roleManagerMock = MockRoleManager();
        roleManagerMock.Setup(roleManager => roleManager
            .FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new ApplicationRole { Id = "1", Name = "Admin", Permissions = Permissions.All });
        roleManagerMock.Setup(roleManager => roleManager
            .UpdateAsync(It.IsAny<ApplicationRole>()))
            .ReturnsAsync(IdentityResult.Success);
        var usersService = new UserService(signInManagerMock.Object, roleManagerMock.Object, new Mock<ILogger<UserService>>().Object);

        // Act
        var result = await usersService.UpdateRole(new RoleRequest("1", "Admin", Permissions.All));

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateRoles_Should_Return_Failiure()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<UserService>>();
        Mock<UserManager<ApplicationUser>> userManagerMock = MockUserManager();
        Mock<SignInManager<ApplicationUser>> signInManagerMock = MockSignInManager(userManagerMock.Object);
        Mock<RoleManager<ApplicationRole>> roleManagerMock = MockRoleManager();
        roleManagerMock.Setup(roleManager => roleManager
            .FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new ApplicationRole { Id = "1", Name = "Admin", Permissions = Permissions.All });
        roleManagerMock.Setup(roleManager => roleManager
            .UpdateAsync(It.IsAny<ApplicationRole>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "Error", Description = "Error" }));
        var usersService = new UserService(signInManagerMock.Object, roleManagerMock.Object, loggerMock.Object);

        // Act
        var result = await usersService.UpdateRole(new RoleRequest("1", "Admin", Permissions.All));

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.First().Message.Should().Be("Could not update role");
        loggerMock.Verify(l =>
        l.Log(LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Could not update role Admin")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
        loggerMock.Verify(l =>
        l.Log(LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Error: Error | Code: Error")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
    }

    [Fact]
    public async Task UpdateRoles_Should_ReturnNotFound_Failiure()
    {
        // Arrange
        Mock<UserManager<ApplicationUser>> userManagerMock = MockUserManager();
        Mock<SignInManager<ApplicationUser>> signInManagerMock = MockSignInManager(userManagerMock.Object);
        Mock<RoleManager<ApplicationRole>> roleManagerMock = MockRoleManager();
        roleManagerMock.Setup(roleManager => roleManager
            .FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(() => null);
        var usersService = new UserService(signInManagerMock.Object, roleManagerMock.Object, new Mock<ILogger<UserService>>().Object);

        // Act
        var result = await usersService.UpdateRole(new RoleRequest("1", "Admin", Permissions.All));

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.First().Message.Should().Be("Role not found");
    }

    [Fact]
    public async Task DeleteRoles_Should_Return_Success()
    {
        // Arrange
        Mock<UserManager<ApplicationUser>> userManagerMock = MockUserManager();
        Mock<SignInManager<ApplicationUser>> signInManagerMock = MockSignInManager(userManagerMock.Object);
        Mock<RoleManager<ApplicationRole>> roleManagerMock = MockRoleManager();
        roleManagerMock.Setup(roleManager => roleManager
            .FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new ApplicationRole { Id = "1", Name = "Admin", Permissions = Permissions.All });
        roleManagerMock.Setup(roleManager => roleManager
            .DeleteAsync(It.IsAny<ApplicationRole>()))
            .ReturnsAsync(IdentityResult.Success);
        var usersService = new UserService(signInManagerMock.Object, roleManagerMock.Object, new Mock<ILogger<UserService>>().Object);

        // Act
        var result = await usersService.DeleteRole("1");

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteRoles_Should_Return_Failiure()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<UserService>>();
        Mock<UserManager<ApplicationUser>> userManagerMock = MockUserManager();
        Mock<SignInManager<ApplicationUser>> signInManagerMock = MockSignInManager(userManagerMock.Object);
        Mock<RoleManager<ApplicationRole>> roleManagerMock = MockRoleManager();
        roleManagerMock.Setup(roleManager => roleManager
            .FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new ApplicationRole { Id = "1", Name = "Admin", Permissions = Permissions.All });
        roleManagerMock.Setup(roleManager => roleManager
            .DeleteAsync(It.IsAny<ApplicationRole>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "Error", Description = "Error" }));
        var usersService = new UserService(signInManagerMock.Object, roleManagerMock.Object, loggerMock.Object);

        // Act
        var result = await usersService.DeleteRole("1");

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.First().Message.Should().Be("Could not delete role");
        loggerMock.Verify(l =>
        l.Log(LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Could not delete role Admin")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
        loggerMock.Verify(l =>
        l.Log(LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Error: Error | Code: Error")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
    }

    [Fact]
    public async Task DeleteRoles_Should_ReturnNotFound_Failiure()
    {
        // Arrange
        Mock<UserManager<ApplicationUser>> userManagerMock = MockUserManager();
        Mock<SignInManager<ApplicationUser>> signInManagerMock = MockSignInManager(userManagerMock.Object);
        Mock<RoleManager<ApplicationRole>> roleManagerMock = MockRoleManager();
        roleManagerMock.Setup(roleManager => roleManager
            .FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(() => null);
        var usersService = new UserService(signInManagerMock.Object, roleManagerMock.Object, new Mock<ILogger<UserService>>().Object);

        // Act
        var result = await usersService.DeleteRole("1");

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.First().Message.Should().Be("Role not found");
    }

    private Mock<RoleManager<ApplicationRole>> MockRoleManager()
    {
        var roles = new List<ApplicationRole>
        {
            new ApplicationRole { Id = "1", Name = "Admin", Permissions = Permissions.All },
            new ApplicationRole { Id = "2", Name = "User", Permissions = Permissions.None },
        };

        var mock = roles.AsQueryable().BuildMock();
        var roleManagerMock = new Mock<RoleManager<ApplicationRole>>(
            new Mock<IRoleStore<ApplicationRole>>().Object,
            new IRoleValidator<ApplicationRole>[0],
            new Mock<ILookupNormalizer>().Object,
            new Mock<IdentityErrorDescriber>().Object,
            new Mock<ILogger<RoleManager<ApplicationRole>>>().Object);

        roleManagerMock.Setup(roleManager => roleManager.Roles).Returns(mock);
        return roleManagerMock;
    }

    private static Mock<UserManager<ApplicationUser>> MockUserManager()
    {
        var users = new List<ApplicationUser>
        {
            new ApplicationUser { Id = "1", UserName = "admin", Email = "admin@localhost.local", FirstName = "Admin", LastName = "User" },
            new ApplicationUser { Id = "2", UserName = "user", Email = "user@localhost.local", FirstName = "User", LastName = "User" },
        };

        var mock = users.AsQueryable().BuildMock();
        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            new Mock<IUserStore<ApplicationUser>>().Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<IPasswordHasher<ApplicationUser>>().Object,
            new IUserValidator<ApplicationUser>[0],
            new IPasswordValidator<ApplicationUser>[0],
            new Mock<ILookupNormalizer>().Object,
            new Mock<IdentityErrorDescriber>().Object,
            new Mock<IServiceProvider>().Object,
            new Mock<ILogger<UserManager<ApplicationUser>>>().Object);

        userManagerMock.Setup(userManager => userManager.Users).Returns(mock);
        return userManagerMock;
    }

    private static Mock<SignInManager<ApplicationUser>> MockSignInManager(UserManager<ApplicationUser> userManager)
    {
        var context = new DefaultHttpContext();
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Name, "admin"),
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Email, "admin@localhost"),
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim(CustomClaimTypes.Permissions, "-1")
        }, "someAuthTypeName"));
        context.User = user;
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.Setup(h => h.HttpContext).Returns(context);

        var signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
            userManager,
            httpContextAccessor.Object,
            new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<ILogger<SignInManager<ApplicationUser>>>().Object,
            new Mock<IAuthenticationSchemeProvider>().Object,
            new Mock<IUserConfirmation<ApplicationUser>>().Object);
        return signInManagerMock;
    }
}
