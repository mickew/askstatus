using System.Security.Claims;
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
        var usersService = new UserService(signInManagerMock.Object, new Mock<ILogger<UserService>>().Object);

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
        var usersService = new UserService(signInManagerMock.Object, new Mock<ILogger<UserService>>().Object);

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
        var usersService = new UserService(signInManagerMock.Object, new Mock<ILogger<UserService>>().Object);

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
        Mock<UserManager<ApplicationUser>> userManagerMock = MockUserManager();
        Mock<SignInManager<ApplicationUser>> signInManagerMock = MockSignInManager(userManagerMock.Object);
        userManagerMock.Setup(userManager => userManager.
            FindByIdAsync(It.IsAny<string>())).
            ReturnsAsync(new ApplicationUser { Id = "1", UserName = "adminb", Email = "adminb@localhost.local", FirstName = "Adminb", LastName = "User" });
        var usersService = new UserService(signInManagerMock.Object, new Mock<ILogger<UserService>>().Object);

        // Act
        var result = await usersService.UpdateUser(new UserRequest("1", "adminb", "adminb@localhost.local", "Adminb", "User"));

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateUser_Should_Return_Failiure()
    {
        // Arrange
        Mock<UserManager<ApplicationUser>> userManagerMock = MockUserManager();
        Mock<SignInManager<ApplicationUser>> signInManagerMock = MockSignInManager(userManagerMock.Object);
        userManagerMock.Setup(userManager => userManager.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(() => null);
        var usersService = new UserService(signInManagerMock.Object, new Mock<ILogger<UserService>>().Object);

        // Act
        var result = await usersService.UpdateUser(new UserRequest("3", "adminb", "adminb@localhost.local", "Adminb", "User"));

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.First().Message.Should().Be("User not found");
    }

    [Fact]
    public async Task CreateUser_Should_Return_Success()
    {
        // Arrange
        Mock<UserManager<ApplicationUser>> userManagerMock = MockUserManager();
        Mock<SignInManager<ApplicationUser>> signInManagerMock = MockSignInManager(userManagerMock.Object);
        userManagerMock.Setup(userManager => userManager.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
        var usersService = new UserService(signInManagerMock.Object, new Mock<ILogger<UserService>>().Object);

        // Act
        var result = await usersService.CreateUser(new UserRequest(string.Empty, "adminb", "adminb@localhost.local", "Adminb", "User"));

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task CreateUser_Should_Return_Failiure()
    {
        // Arrange
        var loggetMock = new Mock<ILogger<UserService>>();
        Mock<UserManager<ApplicationUser>> userManagerMock = MockUserManager();
        Mock<SignInManager<ApplicationUser>> signInManagerMock = MockSignInManager(userManagerMock.Object);
        userManagerMock.Setup(userManager => userManager
            .CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "Error", Description = "Error" }));
        var usersService = new UserService(signInManagerMock.Object, loggetMock.Object);

        // Act
        var result = await usersService.CreateUser(new UserRequest(string.Empty, "adminb", "adminb@localhost.local", "Adminb", "User"));

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.First().Message.Should().Be("Culd not create user");
        //loggetMock.Verify(logger => logger.LogWarning("Could not create user {User}", "adminb"), Times.Once);
        loggetMock.Verify(l =>
        l.Log(LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Could not create user adminb")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
        loggetMock.Verify(l =>
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
        userManagerMock.Setup(userManager => userManager
            .FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new ApplicationUser { Id = "1", UserName = "adminb", Email = "adminb@localhost.local", FirstName = "Adminb", LastName = "User" });
        userManagerMock.Setup(userManager => userManager.DeleteAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);
        var usersService = new UserService(signInManagerMock.Object, new Mock<ILogger<UserService>>().Object);

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
        userManagerMock.Setup(userManager => userManager.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(() => null);
        var usersService = new UserService(signInManagerMock.Object, new Mock<ILogger<UserService>>().Object);

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
        userManagerMock.Setup(userManager => userManager
            .FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new ApplicationUser { Id = "1", UserName = "admin", Email = "admin@localhost.local", FirstName = "Admin", LastName = "User" });
        var usersService = new UserService(signInManagerMock.Object, new Mock<ILogger<UserService>>().Object);

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
        var loggetMock = new Mock<ILogger<UserService>>();
        Mock<UserManager<ApplicationUser>> userManagerMock = MockUserManager();
        Mock<SignInManager<ApplicationUser>> signInManagerMock = MockSignInManager(userManagerMock.Object);
        userManagerMock.Setup(userManager => userManager
            .FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new ApplicationUser { Id = "1", UserName = "adminb", Email = "adminb@localhost.local", FirstName = "Adminb", LastName = "User" });
        userManagerMock.Setup(userManager => userManager
            .DeleteAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "Error", Description = "Error" }));
        var usersService = new UserService(signInManagerMock.Object, loggetMock.Object);

        // Act
        var result = await usersService.DeleteUser("1");

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.First().Message.Should().Be("Could not delete user");
        loggetMock.Verify(l =>
        l.Log(LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Could not delete user adminb")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
        loggetMock.Verify(l =>
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
        userManagerMock.Setup(userManager => userManager
            .FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new ApplicationUser { Id = "1", UserName = "adminb", Email = "adminb@localhost.local", FirstName = "Adminb", LastName = "User" });
        userManagerMock.Setup(userManager => userManager
            .GeneratePasswordResetTokenAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync("token");
        userManagerMock.Setup(userManager => userManager.
            ResetPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        var usersService = new UserService(signInManagerMock.Object, new Mock<ILogger<UserService>>().Object);

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
        userManagerMock.Setup(userManager => userManager.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(() => null);
        var usersService = new UserService(signInManagerMock.Object, new Mock<ILogger<UserService>>().Object);

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
        var loggetMock = new Mock<ILogger<UserService>>();
        Mock<UserManager<ApplicationUser>> userManagerMock = MockUserManager();
        Mock<SignInManager<ApplicationUser>> signInManagerMock = MockSignInManager(userManagerMock.Object);
        userManagerMock.Setup(userManager => userManager
            .FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new ApplicationUser { Id = "1", UserName = "adminb", Email = "adminb@localhost.local", FirstName = "Adminb", LastName = "User" });
        userManagerMock.Setup(userManager => userManager
            .GeneratePasswordResetTokenAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync("token");
        userManagerMock.Setup(userManager => userManager.
            ResetPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "Error", Description = "Error" }));
        var usersService = new UserService(signInManagerMock.Object, loggetMock.Object);

        // Act
        var result = await usersService.ResetPassword("1");

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.First().Message.Should().Be("Could not reset password");
        loggetMock.Verify(l =>
        l.Log(LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Could not reset password for user adminb")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
        loggetMock.Verify(l =>
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
        userManagerMock.Setup(userManager => userManager
            .FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new ApplicationUser { Id = "1", UserName = "adminb", Email = "adminb@localhost.local", FirstName = "Adminb", LastName = "User" });
        userManagerMock.Setup(userManager => userManager
            .ChangePasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        var usersService = new UserService(signInManagerMock.Object, new Mock<ILogger<UserService>>().Object);

        // Act
        var result = await usersService.ChangePassword(new ChangePasswordRequest("OldPassword", "NewPassword"));
    }

    [Fact]
    public async Task ChangePassword_Should_Return_NotFound_Failiure()
    {
        //Arrange
        Mock<UserManager<ApplicationUser>> userManagerMock = MockUserManager();
        Mock<SignInManager<ApplicationUser>> signInManagerMock = MockSignInManager(userManagerMock.Object);
        userManagerMock.Setup(userManager => userManager.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(() => null);
        var usersService = new UserService(signInManagerMock.Object, new Mock<ILogger<UserService>>().Object);

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
        var loggetMock = new Mock<ILogger<UserService>>();
        Mock<UserManager<ApplicationUser>> userManagerMock = MockUserManager();
        Mock<SignInManager<ApplicationUser>> signInManagerMock = MockSignInManager(userManagerMock.Object);
        userManagerMock.Setup(userManager => userManager
            .FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new ApplicationUser { Id = "1", UserName = "adminb", Email = "adminb@localhost.local", FirstName = "Adminb", LastName = "User" });
        userManagerMock.Setup(userManager => userManager
            .ChangePasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "Error", Description = "Error" }));
        var usersService = new UserService(signInManagerMock.Object, loggetMock.Object);

        // Act
        var result = await usersService.ChangePassword(new ChangePasswordRequest("OldPassword", "NewPassword"));

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.First().Message.Should().Be("Could not change password");
        loggetMock.Verify(l =>
        l.Log(LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Could not change password for user adminb")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
        loggetMock.Verify(l =>
        l.Log(LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Error: Error | Code: Error")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
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
