using Askstatus.Application.Identity;
using Askstatus.Application.Interfaces;
using Askstatus.Common.Identity;
using FluentAssertions;
using FluentResults;
using Moq;

namespace Askstatus.Application.Tests
{
    public class IdentityTests
    {
        [Fact]
        public async Task Login_Should_Return_Success()
        {
            // Arrange
            Mock<IIdentityService> mock = new Mock<IIdentityService>();
            mock.Setup(x => x.Login(It.IsAny<LoginRequest>())).ReturnsAsync(Result.Ok());
            LoginUserCommandHandler loginUserCommandHandler = new LoginUserCommandHandler(mock.Object);
            LoginUserCommand loginUserCommand = new LoginUserCommand
            {
                UserName = "admin",
                Password = "admin"
            };

            // Act
            var result = await loginUserCommandHandler.Handle(loginUserCommand, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Login_Should_Return_Failiure()
        {
            // Arrange
            Mock<IIdentityService> mock = new Mock<IIdentityService>();
            mock.Setup(x => x.Login(It.IsAny<LoginRequest>())).ReturnsAsync(Result.Fail("Login failed"));
            LoginUserCommandHandler loginUserCommandHandler = new LoginUserCommandHandler(mock.Object);
            LoginUserCommand loginUserCommand = new LoginUserCommand
            {
                UserName = "admin",
                Password = "admin"
            };

            // Act
            var result = await loginUserCommandHandler.Handle(loginUserCommand, CancellationToken.None);

            // Assert
            result.Errors.Should().NotBeEmpty();
            result.Errors.Should().HaveCount(1);
            result.Errors.First().Message.Should().Be("Login failed");
            result.IsFailed.Should().BeTrue();
        }

        [Fact]
        public async Task LogOut_Should_Return_Success()
        {
            // Arrange
            Mock<IIdentityService> mock = new Mock<IIdentityService>();
            mock.Setup(x => x.Logout()).ReturnsAsync(Result.Ok());
            LogOutUserCommandHandler logOutUserCommandHandler = new LogOutUserCommandHandler(mock.Object);
            LogOutUserCommand logOutUserCommand = new();

            // Act
            var result = await logOutUserCommandHandler.Handle(logOutUserCommand, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task LogOut_Should_Return_Failiure()
        {
            // Arrange
            Mock<IIdentityService> mock = new Mock<IIdentityService>();
            mock.Setup(x => x.Logout()).ReturnsAsync(Result.Fail("Logout failed"));
            LogOutUserCommandHandler logOutUserCommandHandler = new LogOutUserCommandHandler(mock.Object);
            LogOutUserCommand logOutUserCommand = new();

            // Act
            var result = await logOutUserCommandHandler.Handle(logOutUserCommand, CancellationToken.None);

            // Assert
            result.Errors.Should().NotBeEmpty();
            result.Errors.Should().HaveCount(1);
            result.Errors.First().Message.Should().Be("Logout failed");
            result.IsFailed.Should().BeTrue();
        }

        [Fact]
        public async Task GetUserInfo_Should_Return_Success()
        {
            // Arrange
            Mock<IIdentityService> mock = new Mock<IIdentityService>();
            mock.Setup(x => x.GetUserInfo()).ReturnsAsync(Result.Ok(new UserInfoVM("Id", "UseName", "Email")));
            GetUserInfoQueryHandler getUserInfoCommandHandler = new GetUserInfoQueryHandler(mock.Object);
            GetUserInfoQuery getUserInfoCommand = new();

            // Act
            var result = await getUserInfoCommandHandler.Handle(getUserInfoCommand, CancellationToken.None);

            // Assert
            result.Value.Should().NotBeNull();
            result.Value.Id.Should().Be("Id");
            result.Value.UserName.Should().Be("UseName");
            result.Value.Email.Should().Be("Email");
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task GetUserInfo_Should_Return_Failiure()
        {
            // Arrange
            Mock<IIdentityService> mock = new Mock<IIdentityService>();
            mock.Setup(x => x.GetUserInfo()).ReturnsAsync(Result.Fail("User info not found"));
            GetUserInfoQueryHandler getUserInfoCommandHandler = new GetUserInfoQueryHandler(mock.Object);
            GetUserInfoQuery getUserInfoCommand = new();

            // Act
            var result = await getUserInfoCommandHandler.Handle(getUserInfoCommand, CancellationToken.None);

            // Assert
            result.Errors.Should().NotBeEmpty();
            result.Errors.Should().HaveCount(1);
            result.Errors.First().Message.Should().Be("User info not found");
            result.IsFailed.Should().BeTrue();
        }

        [Fact]
        public async Task GetApplicationClaims_Should_Return_Success()
        {
            // Arrange
            var expected = new List<ApplicationClaimVM>
            {
                new ApplicationClaimVM("Issuer", "OriginalIssuer", "Type", "Value", "TypeValue"),
                new ApplicationClaimVM("Issuer", "OriginalIssuer", "Type", "Value", "TypeValue"),
            };
            Mock<IIdentityService> mock = new Mock<IIdentityService>();
            mock.Setup(x => x.GetApplicationClaims()).ReturnsAsync(Result.Ok(expected.AsEnumerable()));
            GetApplicationClaimsQueryHandler getApplicationClaimsCommandHandler = new GetApplicationClaimsQueryHandler(mock.Object);
            GetApplicationClaimsQuery getApplicationClaimsCommand = new();

            // Act
            var result = await getApplicationClaimsCommandHandler.Handle(getApplicationClaimsCommand, CancellationToken.None);

            // Assert
            result.Value.Should().NotBeNull();
            result.Value.Should().HaveCount(2);
            result.Value.Should().BeEquivalentTo(expected.AsEnumerable());
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task GetApplicationClaims_Should_Return_Failiure()
        {
            // Arrange
            Mock<IIdentityService> mock = new Mock<IIdentityService>();
            mock.Setup(x => x.GetApplicationClaims()).ReturnsAsync(Result.Fail("Not authorized"));
            GetApplicationClaimsQueryHandler getApplicationClaimsCommandHandler = new GetApplicationClaimsQueryHandler(mock.Object);
            GetApplicationClaimsQuery getApplicationClaimsCommand = new();

            // Act
            var result = await getApplicationClaimsCommandHandler.Handle(getApplicationClaimsCommand, CancellationToken.None);

            // Assert
            result.Errors.Should().NotBeEmpty();
            result.Errors.Should().HaveCount(1);
            result.Errors.First().Message.Should().Be("Not authorized");
            result.IsFailed.Should().BeTrue();
        }
    }
}
